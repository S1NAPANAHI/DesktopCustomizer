using System;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;

namespace DesktopCustomizer
{
    public partial class SystemInfoWidget : Window
    {
        private DispatcherTimer _timer;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;

        public SystemInfoWidget()
        {
            InitializeComponent();
            InitializeCounters();
            StartTimer();
            SetupWindow();
        }

        private void InitializeCounters()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                
                // Initial reading (often returns 0)
                _cpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing performance counters: {ex.Message}", 
                              "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
            _timer.Tick += UpdateSystemInfo;
            _timer.Start();
        }

        private void UpdateSystemInfo(object? sender, EventArgs e)
        {
            try
            {
                // CPU Usage
                float cpuUsage = _cpuCounter?.NextValue() ?? 0;
                CpuText.Text = $"CPU: {cpuUsage:F1}%";
                CpuProgress.Value = cpuUsage;

                // RAM Usage
                float availableRAM = _ramCounter?.NextValue() ?? 0;
                float totalRAM = GetTotalRAM();
                float usedRAM = totalRAM - availableRAM;
                float ramPercentage = totalRAM > 0 ? (usedRAM / totalRAM) * 100 : 0;
                
                RamText.Text = $"RAM: {ramPercentage:F1}% ({usedRAM:F0}/{totalRAM:F0} MB)";
                RamProgress.Value = ramPercentage;

                // Disk Usage (C: drive)
                var driveInfo = new System.IO.DriveInfo("C:");
                if (driveInfo.IsReady)
                {
                    long totalBytes = driveInfo.TotalSize;
                    long freeBytes = driveInfo.TotalFreeSpace;
                    long usedBytes = totalBytes - freeBytes;
                    double diskPercentage = (double)usedBytes / totalBytes * 100;
                    
                    DiskText.Text = $"Disk: {diskPercentage:F1}% ({usedBytes / (1024*1024*1024):F0}/{totalBytes / (1024*1024*1024):F0} GB)";
                    DiskProgress.Value = diskPercentage;
                }
            }
            catch (Exception ex)
            {
                // Handle errors silently for now
                Debug.WriteLine($"Error updating system info: {ex.Message}");
            }
        }

        private float GetTotalRAM()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return (float)(Convert.ToDouble(obj["TotalPhysicalMemory"]) / (1024 * 1024));
                }
            }
            catch
            {
                // Fallback estimation
                return 8192; // Assume 8GB
            }
            return 0;
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
                
                // Apply to progress bars
                CpuProgress.Foreground = primaryBrush;
                RamProgress.Foreground = primaryBrush;
                DiskProgress.Foreground = primaryBrush;
                
                // Show/Hide widget
                this.Visibility = settings.ShowSystemWidget ? Visibility.Visible : Visibility.Hidden;
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
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            base.OnClosed(e);
        }
    }
}