using System;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace DesktopCustomizer
{
    public partial class SettingsWindow : Window
    {
        private SettingsManager _settingsManager;
        private AppSettings _currentSettings;
        
        public event EventHandler? SettingsChanged;

        public SettingsWindow(SettingsManager settingsManager)
        {
            InitializeComponent();
            _settingsManager = settingsManager;
            _currentSettings = _settingsManager.LoadSettings();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            try
            {
                // Load color settings
                PrimaryColorText.Text = _currentSettings.PrimaryColor;
                BackgroundColorText.Text = _currentSettings.BackgroundColor;
                
                // Load opacity
                OpacitySlider.Value = _currentSettings.Opacity * 100;
                OpacityValue.Text = $"{_currentSettings.Opacity * 100:F0}%";
                
                // Load widget visibility
                ShowSystemWidget.IsChecked = _currentSettings.ShowSystemWidget;
                ShowClockWidget.IsChecked = _currentSettings.ShowClockWidget;
                
                // Update preview
                UpdatePreview();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OpacityValue != null)
            {
                OpacityValue.Text = $"{e.NewValue:F0}%";
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            try
            {
                var primaryColor = PrimaryColorText?.Text ?? "#0078D4";
                var backgroundColor = BackgroundColorText?.Text ?? "#1E1E1E";
                
                // Update preview background
                var backgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundColor));
                PreviewBorder.Background = backgroundBrush;
                
                // Update preview accent color
                var primaryBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(primaryColor));
                PreviewProgress.Foreground = primaryBrush;
                
                // Update opacity
                if (OpacitySlider != null)
                {
                    PreviewBorder.Opacity = OpacitySlider.Value / 100.0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating preview: {ex.Message}");
            }
        }

        private void ColorTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update settings
                _currentSettings.PrimaryColor = PrimaryColorText.Text;
                _currentSettings.BackgroundColor = BackgroundColorText.Text;
                _currentSettings.Opacity = OpacitySlider.Value / 100.0;
                _currentSettings.ShowSystemWidget = ShowSystemWidget.IsChecked ?? true;
                _currentSettings.ShowClockWidget = ShowClockWidget.IsChecked ?? true;
                
                // Save settings
                _settingsManager.SaveSettings(_currentSettings);
                
                // Notify that settings changed
                SettingsChanged?.Invoke(this, EventArgs.Empty);
                
                MessageBox.Show("Settings applied successfully!", "Settings", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying settings: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentSettings = new AppSettings();
                LoadCurrentSettings();
                UpdatePreview();
                
                MessageBox.Show("Settings reset to defaults!", "Settings", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting settings: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}