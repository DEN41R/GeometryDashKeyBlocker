using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class MainWindow : Form
{
    private readonly Label _statusLabel;
    private readonly Button _toggleButton;
    private readonly Label _infoLabel;
    private readonly Config _config;
    private Process? _geometryDashProcess;
    private SettingsForm? _settingsForm;

    public MainWindow(Config config)
    {
        _config = config;
        
        Text = "GD Key Blocker";
        Size = new Size(350, 250);
        MinimumSize = new Size(350, 250);
        FormBorderStyle = FormBorderStyle.Sizable;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = true;
        MinimizeBox = true;
        TopMost = _config.AlwaysOnTop;
        Icon = SystemIcons.Application;


        _statusLabel = new Label
        {
            Text = "Ожидание GD...",
            ForeColor = Color.Gray,
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 20)
        };


        _toggleButton = new Button
        {
            Text = "БЛОКИРОВКА ВКЛЮЧЕНА",
            Size = new Size(310, 50),
            Location = new Point(20, 50),
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            BackColor = Color.FromArgb(46, 160, 67),
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 }
        };
        _toggleButton.Click += (s, e) => SetBlocking(!_isBlocking);

       
        _infoLabel = new Label
        {
            Text = GetInfoText(),
            ForeColor = Color.Black,
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 120)
        };

    
        var settingsButton = new Button
        {
            Text = "Настройки",
            Size = new Size(310, 40),
            Location = new Point(20, 160),
            Font = new Font("Segoe UI", 10),
            FlatStyle = FlatStyle.System
        };
        settingsButton.Click += (s, e) => ShowSettings();

     
        Controls.AddRange(new Control[] { 
            _statusLabel,
            _toggleButton,
            _infoLabel,
            settingsButton
        });


        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) =>
        {
            var processes = Process.GetProcessesByName(_config.ProcessName);
            if (processes.Length > 0)
            {
                if (_geometryDashProcess == null || _geometryDashProcess.HasExited)
                {
                    _geometryDashProcess = processes[0];
                    _statusLabel.Text = "GD запущен";
                    _statusLabel.ForeColor = Color.Green;
                }
            }
            else
            {
                _geometryDashProcess = null;
                _statusLabel.Text = "Ожидание GD...";
                _statusLabel.ForeColor = Color.Gray;
            }
        };
        timer.Start();
    }

    private string GetInfoText()
    {
        return $"Клавиша: {_config.BlockedKeyString}\nВкл/Выкл: {_config.ToggleHotkeyString}";
    }

    public void UpdateInfo()
    {
        _infoLabel.Text = GetInfoText();
        TopMost = _config.AlwaysOnTop;
    }

    private bool _isBlocking = true;
    public bool IsBlocking => _isBlocking;

    public void ShowSettings()
    {
        if (_settingsForm == null || _settingsForm.IsDisposed)
        {
            _settingsForm = new SettingsForm(_config);
            _settingsForm.Owner = this;
            _settingsForm.SettingsChanged += (s, e) => UpdateInfo();
        }

        if (!_settingsForm.Visible)
        {
            _settingsForm.Show();
            if (_config.AlwaysOnTop)
            {
                _settingsForm.TopMost = true;
            }
        }
        else
        {
            _settingsForm.Focus();
        }
    }

    public void SetBlocking(bool blocking)
    {
        _isBlocking = blocking;
        _toggleButton.Text = blocking ? "БЛОКИРОВКА ВКЛЮЧЕНА" : "БЛОКИРОВКА ОТКЛЮЧЕНА";
        _toggleButton.BackColor = blocking ? Color.FromArgb(46, 160, 67) : Color.FromArgb(200, 55, 55);
    }
}

 