using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

public static class Program
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

    private static MainWindow? _mainWindow;
    private static TrayIcon? _trayIcon;
    private static LowLevelKeyboardHook? _keyboardHook;
    private static Config? _config;
    private static Process? _geometryDashProcess;
    private static System.Threading.Timer? _processMonitorTimer;

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        try
        {
            
            _config = Config.Load();
            if (_config == null)
            {
                MessageBox.Show(
                    "Не удалось загрузить конфигурацию. Приложение будет закрыто.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

         
            _mainWindow = new MainWindow(_config);
            if (_mainWindow == null)
            {
                MessageBox.Show(
                    "Не удалось создать главное окно. Приложение будет закрыто.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            _mainWindow.TopMost = _config.AlwaysOnTop;

        
            _trayIcon = new TrayIcon(_mainWindow);

         
            _keyboardHook = new LowLevelKeyboardHook(_mainWindow, _config);

      
            _processMonitorTimer = new System.Threading.Timer(
                MonitorProcess,
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1)
            );

            Application.Run();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Произошла ошибка: {ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private static void MonitorProcess(object? state)
    {
        try
        {
            var processes = Process.GetProcessesByName("GeometryDash");
            if (processes.Length > 0)
            {
                if (_geometryDashProcess == null || _geometryDashProcess.HasExited)
                {
                    _geometryDashProcess = processes[0];
                }
            }
            else
            {
                _geometryDashProcess = null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка мониторинга процесса: {ex.Message}");
        }
    }
}
