using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Diagnostics;

namespace DesktopCustomizer
{
    public partial class ClockWidget : Window
    {
        private DispatcherTimer _timer;

        public ClockWidget()
        {
            InitializeComponent();
            SetupWindow();
            StartTimer();
        }

        private void SetupWindow()
        {
            // Make window stay on desktop
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            
            // Make window draggable
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            
            // Context menu for settings
            var contextMenu = new System.Windows.Controls.ContextMenu();
            var settingsItem = new System.Windows.Controls.MenuItem() { Header = "Settings" };
            settingsItem.Click += (s, e) => ShowSettings();
            contextMenu.Items.Add(settingsItem);
            
            this.ContextMenu = contextMenu;
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += UpdateTime;
            _timer.Start();
            
            // Update immediately
            UpdateTime(null, null);
        }

        private void UpdateTime(object? sender, EventArgs e)
        {
            try
            {
                var now = DateTime.Now;
                TimeText.Text = now.ToString("HH:mm:ss");
                DateText.Text = now.ToString("dddd, MMMM dd, yyyy");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating time: {ex.Message}");
            }
        }

        public void ApplySettings(AppSettings settings)
        {
            try
            {
                // Apply colors
                var primaryBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.PrimaryColor));
                var backgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(settings.BackgroundColor));
                
                MainBorder.Background = backgroundBrush;
                this.Opacity = settings.Opacity;
                
                // Apply to text (optional: could use primary color for accents)
                TimeText.Foreground = Brushes.White;
                DateText.Foreground = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)); // Semi-transparent white
                
                // Show/Hide widget
                this.Visibility = settings.ShowClockWidget ? Visibility.Visible : Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying settings: {ex.Message}");
            }
        }

        private void ShowSettings()
        {
            var settingsWindow = new SettingsWindow(new SettingsManager());
            settingsWindow.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}