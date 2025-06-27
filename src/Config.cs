using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

public class Config
{
    private static readonly string ConfigPath = "config.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string ProcessName { get; set; } = "GeometryDash";
    public uint BlockedKey { get; set; } = (uint)Keys.Space;
    public string BlockedKeyString { get; set; } = "Пробел";
    public uint ToggleHotkey { get; set; } = (uint)Keys.OemPeriod;
    public string ToggleHotkeyString { get; set; } = ".";
    public bool AlwaysOnTop { get; set; } = true;

    public static Config Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var config = JsonSerializer.Deserialize<Config>(json, JsonOptions);
                if (config != null)
                {
                    Logger.Info("Конфигурация загружена");
                    return config;
                }
            }
            else
            {
                Logger.Info("Файл конфигурации не найден, создаем новый");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Ошибка загрузки конфигурации: {ex.Message}");
            Logger.Info("Используем настройки по умолчанию");
        }

        var defaultConfig = new Config();
        defaultConfig.Save();
        return defaultConfig;
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, JsonOptions);
            var tempPath = ConfigPath + ".tmp";

         
            File.WriteAllText(tempPath, json);

      
            if (File.Exists(ConfigPath))
            {
                File.Delete(ConfigPath);
            }
            File.Move(tempPath, ConfigPath);

            Logger.Info("Конфигурация сохранена");
        }
        catch (Exception ex)
        {
            Logger.Error($"Ошибка сохранения конфигурации: {ex.Message}");
        }
    }
} 