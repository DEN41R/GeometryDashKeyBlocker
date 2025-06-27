using System.Text;

public static class Logger
{
    private static readonly string LogPath = "keyblock.log";
    private static readonly string OldLogPath = "keyblock.old.log";
    private static readonly long MaxLogSize = 1024 * 1024; // 1 MB
    private static bool _enabled = true;
    private static LogLevel _currentLevel = LogLevel.Info;
    private static readonly object _lockObject = new();

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public static void SetEnabled(bool enabled)
    {
        _enabled = enabled;
        Info($"Логирование {(_enabled ? "включено" : "отключено")}");
    }

    public static void SetLevel(LogLevel level)
    {
        _currentLevel = level;
        Info($"Установлен уровень логирования: {level}");
    }

    public static void Debug(string message)
    {
        if (_currentLevel <= LogLevel.Debug)
        {
            Log("DEBUG", message);
        }
    }

    public static void Info(string message)
    {
        if (_currentLevel <= LogLevel.Info)
        {
            Log("INFO", message);
        }
    }

    public static void Warning(string message)
    {
        if (_currentLevel <= LogLevel.Warning)
        {
            Log("WARN", message);
        }
    }

    public static void Error(string message)
    {
        if (_currentLevel <= LogLevel.Error)
        {
            Log("ERROR", message);
        }
    }

    public static void Clear()
    {
        try
        {
            lock (_lockObject)
            {
                if (File.Exists(LogPath))
                {
                    File.Delete(LogPath);
                }
                if (File.Exists(OldLogPath))
                {
                    File.Delete(OldLogPath);
                }
                Info("Лог очищен");
            }
        }
        catch (Exception ex)
        {
            Error($"Ошибка очистки лога: {ex.Message}");
        }
    }

    private static void Log(string level, string message)
    {
        if (!_enabled) return;

        try
        {
            lock (_lockObject)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logMessage = $"[{timestamp}] [{level}] {message}{Environment.NewLine}";

        
                if (File.Exists(LogPath))
                {
                    var fileInfo = new FileInfo(LogPath);
                    if (fileInfo.Length > MaxLogSize)
                    {
                
                        if (File.Exists(OldLogPath))
                        {
                            File.Delete(OldLogPath);
                        }

                       
                        File.Move(LogPath, OldLogPath);

             
                        var rotationMessage = $"[{timestamp}] [INFO] Выполнена ротация лога{Environment.NewLine}";
                        File.WriteAllText(LogPath, rotationMessage, Encoding.UTF8);
                    }
                }

            
                File.AppendAllText(LogPath, logMessage, Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
         
            Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
            Console.WriteLine($"Исходное сообщение: [{level}] {message}");
        }
    }

    public static string[] GetLastLines(int count = 100)
    {
        try
        {
            lock (_lockObject)
            {
                if (!File.Exists(LogPath))
                {
                    return Array.Empty<string>();
                }

                var lines = new List<string>();
                var allLines = File.ReadAllLines(LogPath);
                
            
                if (allLines.Length < count && File.Exists(OldLogPath))
                {
                    var oldLines = File.ReadAllLines(OldLogPath);
                    lines.AddRange(oldLines.Skip(Math.Max(0, oldLines.Length - (count - allLines.Length))));
                }

                lines.AddRange(allLines.Skip(Math.Max(0, allLines.Length - count)));
                return lines.ToArray();
            }
        }
        catch (Exception ex)
        {
            Error($"Ошибка чтения лога: {ex.Message}");
            return Array.Empty<string>();
        }
    }
} 