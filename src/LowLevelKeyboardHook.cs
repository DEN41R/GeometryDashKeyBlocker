using System.Runtime.InteropServices;
using System.Diagnostics;

public class LowLevelKeyboardHook : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_LBUTTONUP = 0x0202;

    private readonly MainWindow _mainWindow;
    private readonly Config _config;
    private readonly IntPtr _keyboardHookID;
    private readonly IntPtr _mouseHookID;
    private readonly LowLevelKeyboardProc _keyboardProc;
    private readonly LowLevelMouseProc _mouseProc;
    private Process? _geometryDashProcess;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

    public LowLevelKeyboardHook(MainWindow mainWindow, Config config)
    {
        _mainWindow = mainWindow;
        _config = config;

        _keyboardProc = KeyboardHookCallback;
        _mouseProc = MouseHookCallback;

        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule;
        if (module != null)
        {
            var moduleHandle = GetModuleHandle(module.ModuleName);
            _keyboardHookID = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, moduleHandle, 0);
            _mouseHookID = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, moduleHandle, 0);
            Logger.Info("Хуки клавиатуры и мыши установлены");
        }


        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) => MonitorProcess();
        timer.Start();
    }

    private void MonitorProcess()
    {
        try
        {
            var processes = Process.GetProcessesByName(_config.ProcessName);
            if (processes.Length > 0)
            {
                if (_geometryDashProcess == null || _geometryDashProcess.HasExited)
                {
                    _geometryDashProcess = processes[0];
                    Logger.Info($"Процесс {_config.ProcessName} найден (PID: {_geometryDashProcess.Id})");
                }
            }
            else
            {
                if (_geometryDashProcess != null)
                {
                    Logger.Info($"Процесс {_config.ProcessName} закрыт");
                    _geometryDashProcess = null;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Ошибка при мониторинге процесса: {ex}");
        }
    }

    private bool IsGeometryDashWindow(IntPtr windowHandle)
    {
        if (_geometryDashProcess == null) return false;

        GetWindowThreadProcessId(windowHandle, out int processId);
        return processId == _geometryDashProcess.Id;
    }

    private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var foregroundWindow = GetForegroundWindow();
            
           
            var vkCode = Marshal.ReadInt32(lParam);
            if (_config.ToggleHotkey != 0 && vkCode == _config.ToggleHotkey && 
                (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                _mainWindow.SetBlocking(!_mainWindow.IsBlocking);
                Logger.Info($"Переключение блокировки через горячую клавишу: {(_mainWindow.IsBlocking ? "вкл" : "выкл")}");
                return (IntPtr)1;
            }

     
            if (_mainWindow.IsBlocking && IsGeometryDashWindow(foregroundWindow))
            {
                if (_config.BlockedKey == (uint)vkCode)
                {
                    if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                    {
                        Logger.Debug($"Заблокирована клавиша: {(System.Windows.Forms.Keys)vkCode}");
                        return (IntPtr)1;
                    }
                }
            }
        }

        return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
    }

    private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var foregroundWindow = GetForegroundWindow();

          
            if (_mainWindow.IsBlocking && IsGeometryDashWindow(foregroundWindow))
            {
                if (_config.BlockedKey == (uint)System.Windows.Forms.Keys.LButton)
                {
                    if (wParam == (IntPtr)WM_LBUTTONDOWN)
                    {
                        Logger.Debug("Заблокирован клик ЛКМ");
                        return (IntPtr)1;
                    }
                }
            }
        }

        return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        UnhookWindowsHookEx(_keyboardHookID);
        UnhookWindowsHookEx(_mouseHookID);
        Logger.Info("Хуки клавиатуры и мыши удалены");
    }
} 