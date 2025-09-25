using System;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel;

namespace DesktopCustomizer
{
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private SystemInfoWidget? _systemWidget;
        private ClockWidget? _clockWidget;
        private SettingsManager _settingsManager;

        public App()
        {
            _settingsManager = new SettingsManager();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Create system tray icon
            CreateNotifyIcon();
            
            // Load and create widgets
            LoadWidgets();
        }

        private void CreateNotifyIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            _notifyIcon.Text = "Desktop Customizer";
            _notifyIcon.Visible = true;
            
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Settings", null, (s, e) => ShowSettings());
            contextMenu.Items.Add("Exit", null, (s, e) => Shutdown());
            
            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => ShowSettings();
        }

        private void LoadWidgets()
        {
            var settings = _settingsManager.LoadSettings();
            
            // Create System Info Widget
            _systemWidget = new SystemInfoWidget();
            _systemWidget.Left = settings.SystemWidgetX;
            _systemWidget.Top = settings.SystemWidgetY;
            _systemWidget.Show();
            
            // Create Clock Widget
            _clockWidget = new ClockWidget();
            _clockWidget.Left = settings.ClockWidgetX;
            _clockWidget.Top = settings.ClockWidgetY;
            _clockWidget.Show();
        }

        private void ShowSettings()
        {
            var settingsWindow = new SettingsWindow(_settingsManager);
            settingsWindow.SettingsChanged += OnSettingsChanged;
            settingsWindow.Show();
        }

        private void OnSettingsChanged(object? sender, EventArgs e)
        {
            // Refresh widgets with new settings
            var settings = _settingsManager.LoadSettings();
            
            _systemWidget?.ApplySettings(settings);
            _clockWidget?.ApplySettings(settings);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Save widget positions
            if (_systemWidget != null && _clockWidget != null)
            {
                var settings = _settingsManager.LoadSettings();
                settings.SystemWidgetX = _systemWidget.Left;
                settings.SystemWidgetY = _systemWidget.Top;
                settings.ClockWidgetX = _clockWidget.Left;
                settings.ClockWidgetY = _clockWidget.Top;
                _settingsManager.SaveSettings(settings);
            }

            _notifyIcon?.Dispose();
            base.OnExit(e);
        }
    }

    // Settings Data Model
    public class AppSettings
    {
        public double SystemWidgetX { get; set; } = 50;
        public double SystemWidgetY { get; set; } = 50;
        public double ClockWidgetX { get; set; } = 300;
        public double ClockWidgetY { get; set; } = 50;
        public string PrimaryColor { get; set; } = "#0078D4";
        public string BackgroundColor { get; set; } = "#1E1E1E";
        public double Opacity { get; set; } = 0.9;
        public bool ShowSystemWidget { get; set; } = true;
        public bool ShowClockWidget { get; set; } = true;
    }

    // Settings Manager
    public class SettingsManager
    {
        private readonly string _settingsPath;

        public SettingsManager()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appData, "DesktopCustomizer");
            Directory.CreateDirectory(appFolder);
            _settingsPath = Path.Combine(appFolder, "settings.json");
        }

        public AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch
            {
                // If loading fails, return default settings
            }
            
            return new AppSettings();
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch
            {
                // Handle save errors silently for now
            }
        }
    }
}