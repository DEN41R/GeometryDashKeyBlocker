using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class SettingsForm : Form
{
    private readonly Config _config;
    private readonly ComboBox _keyComboBox;
    private readonly Button _hotkeyButton;
    private readonly CheckBox _alwaysOnTopCheckBox;
    private bool _isCapturingHotkey = false;

    public event EventHandler? SettingsChanged;

    private static readonly Dictionary<string, Keys> KeyMap = new()
    {
        { "Пробел", Keys.Space },
        { "ЛКМ", Keys.LButton },
        { "Стрелка вверх", Keys.Up }
    };

    public SettingsForm(Config config)
    {
        _config = config;

        Text = "Настройки";
        Size = new Size(350, 300);
        MinimumSize = new Size(350, 300);
        FormBorderStyle = FormBorderStyle.Sizable;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = true;
        MinimizeBox = true;
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

      
        var keyLabel = new Label
        {
            Text = "Блокируемая клавиша:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10),
            Location = new Point(20, 20)
        };

        _keyComboBox = new ComboBox
        {
            Width = 310,
            Height = 30,
            Location = new Point(20, 45),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _keyComboBox.Items.AddRange(KeyMap.Keys.ToArray());
        _keyComboBox.SelectedItem = _config.BlockedKeyString;

        
        var hotkeyLabel = new Label
        {
            Text = "Горячая клавиша вкл/выкл:",
            AutoSize = true,
            Font = new Font("Segoe UI", 10),
            Location = new Point(20, 90)
        };

        _hotkeyButton = new Button
        {
            Text = _config.ToggleHotkey == 0 ? "Нажмите для назначения" : $"Назначено: {_config.ToggleHotkeyString}",
            Width = 310,
            Height = 40,
            Location = new Point(20, 115),
            Font = new Font("Segoe UI", 10)
        };
        _hotkeyButton.Click += (s, e) =>
        {
            _isCapturingHotkey = true;
            _hotkeyButton.Text = "Нажмите любую клавишу...";
            _hotkeyButton.BackColor = Color.LightGray;
        };

      
        _alwaysOnTopCheckBox = new CheckBox
        {
            Text = "Поверх других окон",
            AutoSize = true,
            Font = new Font("Segoe UI", 10),
            Location = new Point(20, 170),
            Checked = _config.AlwaysOnTop
        };

  
        var saveButton = new Button
        {
            Text = "Сохранить",
            Size = new Size(150, 40),
            Location = new Point(20, 210),
            Font = new Font("Segoe UI", 10),
            FlatStyle = FlatStyle.System
        };
        saveButton.Click += (s, e) => 
        {
            SaveSettings();
            Close();
        };

        var cancelButton = new Button
        {
            Text = "Отмена",
            Size = new Size(150, 40),
            Location = new Point(180, 210),
            Font = new Font("Segoe UI", 10),
            FlatStyle = FlatStyle.System
        };
        cancelButton.Click += (s, e) => Close();

   
        Controls.AddRange(new Control[] {
            keyLabel,
            _keyComboBox,
            hotkeyLabel,
            _hotkeyButton,
            _alwaysOnTopCheckBox,
            saveButton,
            cancelButton
        });

    
        KeyPreview = true;
        KeyDown += (s, e) =>
        {
            if (_isCapturingHotkey)
            {
                _config.ToggleHotkey = (uint)e.KeyCode;
                _config.ToggleHotkeyString = e.KeyCode.ToString();
                _hotkeyButton.Text = $"Назначено: {_config.ToggleHotkeyString}";
                _hotkeyButton.BackColor = SystemColors.Control;
                _isCapturingHotkey = false;
                e.Handled = true;
            }
        };


        _keyComboBox.SelectedIndexChanged += (s, e) => SaveSettings();
        _alwaysOnTopCheckBox.CheckedChanged += (s, e) => SaveSettings();
    }

    private void SaveSettings()
    {
        if (_keyComboBox.SelectedItem != null)
        {
            var keyString = _keyComboBox.SelectedItem.ToString();
            if (keyString != null && KeyMap.ContainsKey(keyString))
            {
                _config.BlockedKey = (uint)KeyMap[keyString];
                _config.BlockedKeyString = keyString;
            }
        }
        _config.AlwaysOnTop = _alwaysOnTopCheckBox.Checked;
        _config.Save();

        
        SettingsChanged?.Invoke(this, EventArgs.Empty);

      
        if (Owner is MainWindow mainWindow)
        {
            mainWindow.TopMost = _config.AlwaysOnTop;
        }
    }
} 