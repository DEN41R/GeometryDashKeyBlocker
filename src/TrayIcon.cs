using System.Drawing;
using System.Windows.Forms;

public class TrayIcon : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly MainWindow _mainWindow;
    private readonly ToolStripMenuItem _toggleItem;
    private readonly ToolStripMenuItem _statusItem;

    public TrayIcon(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        
        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Geometry Dash Key Blocker",
            Visible = true
        };

   
        var contextMenu = new ContextMenuStrip
        {
            BackColor = Color.FromArgb(28, 28, 28),
            ForeColor = Color.White,
            RenderMode = ToolStripRenderMode.System,
            ShowImageMargin = false
        };

      
        _statusItem = new ToolStripMenuItem("Статус: Ожидание игры...")
        {
            Enabled = false,
            Font = new Font("Segoe UI", 9, FontStyle.Regular)
        };

     
        _toggleItem = new ToolStripMenuItem("Отключить блокировку")
        {
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        _toggleItem.Click += (s, e) =>
        {
            bool newState = !_mainWindow.IsBlocking;
            _mainWindow.SetBlocking(newState);
            UpdateMenuItems(newState);
            ShowNotification(
                "Geometry Dash Key Blocker",
                newState ? "Блокировка включена" : "Блокировка отключена",
                newState ? ToolTipIcon.Info : ToolTipIcon.Warning
            );
        };

       
        var showHideItem = new ToolStripMenuItem("Показать окно")
        {
            Font = new Font("Segoe UI", 9)
        };
        showHideItem.Click += (s, e) =>
        {
            if (_mainWindow.Visible)
            {
                _mainWindow.Hide();
                showHideItem.Text = "Показать окно";
            }
            else
            {
                _mainWindow.Show();
                _mainWindow.BringToFront();
                _mainWindow.Activate();
                showHideItem.Text = "Скрыть окно";
            }
        };

      
        var settingsItem = new ToolStripMenuItem("Настройки")
        {
            Font = new Font("Segoe UI", 9)
        };
        settingsItem.Click += (s, e) =>
        {
            _mainWindow.Show();
            _mainWindow.BringToFront();
            _mainWindow.Activate();
            _mainWindow.ShowSettings();
        };

    
        var separator = new ToolStripSeparator();

       
        var exitItem = new ToolStripMenuItem("Выход")
        {
            Font = new Font("Segoe UI", 9)
        };
        exitItem.Click += (s, e) => Application.Exit();


        contextMenu.Items.AddRange(new ToolStripItem[]
        {
            _statusItem,
            _toggleItem,
            new ToolStripSeparator(),
            showHideItem,
            settingsItem,
            separator,
            exitItem
        });

    
        foreach (ToolStripItem item in contextMenu.Items)
        {
            if (item is ToolStripMenuItem menuItem)
            {
                menuItem.BackColor = Color.FromArgb(28, 28, 28);
                menuItem.ForeColor = Color.White;
                
                menuItem.MouseEnter += (s, e) =>
                {
                    if (menuItem.Enabled)
                        menuItem.BackColor = Color.FromArgb(45, 45, 45);
                };
                
                menuItem.MouseLeave += (s, e) =>
                {
                    if (menuItem.Enabled)
                        menuItem.BackColor = Color.FromArgb(28, 28, 28);
                };
            }
        }

        _notifyIcon.ContextMenuStrip = contextMenu;

      
        _notifyIcon.DoubleClick += (s, e) =>
        {
            _mainWindow.Show();
            _mainWindow.BringToFront();
            _mainWindow.Activate();
        };

      
        var statusTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        statusTimer.Tick += (s, e) =>
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("GeometryDash");
            _statusItem.Text = processes.Length > 0 ?
                "Статус: Игра запущена" :
                "Статус: Ожидание игры...";
            _statusItem.ForeColor = processes.Length > 0 ?
                Color.FromArgb(46, 160, 67) :
                Color.Gray;
        };
        statusTimer.Start();
    }

    public void UpdateMenuItems(bool isBlocking)
    {
        _toggleItem.Text = isBlocking ? "Отключить блокировку" : "Включить блокировку";
        _toggleItem.ForeColor = isBlocking ? Color.FromArgb(46, 160, 67) : Color.FromArgb(200, 55, 55);
    }

    public void ShowNotification(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
    {
        _notifyIcon.ShowBalloonTip(3000, title, text, icon);
    }

    public void Dispose()
    {
        _notifyIcon.Dispose();
    }
} 