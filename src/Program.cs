using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBOARD_INPUT
    {
        public uint type;
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, KEYBOARD_INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int VK_UP = 0x26;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint INPUT_KEYBOARD = 1;
    private static Process? _geometryDashProcess = null;
    private static bool _isRunning = true;
    private static IntPtr _consoleHandle;
    private static IntPtr _hookHandle;
    private static LowLevelKeyboardProc _proc;

    static async Task Main(string[] args)
    {
        _consoleHandle = Process.GetCurrentProcess().MainWindowHandle;
        
        Console.WriteLine("Ищем процесс GeometryDash.exe...");
        
        while (true)
        {
            var processes = Process.GetProcessesByName("GeometryDash");
            if (processes.Length > 0)
            {
                _geometryDashProcess = processes[0];
                break;
            }
            Thread.Sleep(1000);
            Console.WriteLine("Ожидание запуска GeometryDash.exe...");
        }

        Console.WriteLine("Процесс GeometryDash.exe найден!");
        Console.WriteLine("Начинаю блокировку клавиши стрелки вверх...");

     
        _proc = new LowLevelKeyboardProc(HookCallback);
        _hookHandle = SetHook(_proc);

    
        RegisterHotKey(_consoleHandle, 1, 0, (uint)VK_UP);

        var monitorTask = Task.Run(MonitorKeyPress);

        Console.WriteLine("Блокировка активна. Нажмите Enter для выхода.");
        Console.ReadLine();


        UnregisterHotKey(_consoleHandle, 1);
        UnhookWindowsHookEx(_hookHandle);
        _isRunning = false;
        await monitorTask;
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule? curModule = curProcess.MainModule)
        {
            if (curModule != null)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
            return IntPtr.Zero;
        }
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && IsGeometryDashActive())
        {
            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == VK_UP)
                {
                    BlockUpKey();
                    return (IntPtr)1;
                }
            }
        }
        return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }

    private static bool IsGeometryDashActive()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        uint processId;
        GetWindowThreadProcessId(foregroundWindow, out processId);

        return _geometryDashProcess != null && 
               !_geometryDashProcess.HasExited && 
               _geometryDashProcess.Id == processId;
    }

    private static void BlockUpKey()
    {
        KEYBOARD_INPUT[] inputs = new KEYBOARD_INPUT[1];
        inputs[0] = new KEYBOARD_INPUT
        {
            type = INPUT_KEYBOARD,
            wVk = (ushort)VK_UP,
            dwFlags = KEYEVENTF_KEYUP,
            time = 0,
            dwExtraInfo = IntPtr.Zero
        };

        SendInput(1, inputs, Marshal.SizeOf(typeof(KEYBOARD_INPUT)));
    }

    private static async Task MonitorKeyPress()
    {
        while (_isRunning)
        {
            if (IsGeometryDashActive())
            {
                BlockUpKey();
            }
            await Task.Delay(1);
        }
    }
}
