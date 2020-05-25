using ColorPickerWPF;
using FullScreenStats.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfScreenHelper;

namespace FullScreenStats {
    /// <summary>
    /// Interaction logic for MonitorSettings.xaml
    /// </summary>
    public partial class MonitorSettings : Window {
        MainWindow mainWindow;
        List<Screen> connectedScreens = new List<Screen>();
        List<IdentifierWindow> identifierWindows = new List<IdentifierWindow>();
        bool showingIdentifier = false;
        public MonitorSettings(MainWindow window) {
            InitializeComponent();
            mainWindow = window;
            foreach (Screen screen in Screen.AllScreens) {
                lstBox_monitors.Items.Add(screen.DeviceName);
                connectedScreens.Add(screen);
            }
            txt_fontSize.Text = Settings.Default.global_font_size.ToString();
            txt_fontStyle.Text = Settings.Default.global_font;
        }

        private void btn_closeMonitorSettings_Click(object sender, RoutedEventArgs e) {
            if (showingIdentifier) {
                foreach (IdentifierWindow window in identifierWindows) {
                    window.Close();
                }
                identifierWindows.Clear();
                showingIdentifier = false;
            }
            if(lstBox_monitors.SelectedItems.Count > 0) {
                mainWindow.txt_selectedMonitors.Text = "";
            }
            foreach (String screenID in lstBox_monitors.SelectedItems) {
                mainWindow.txt_selectedMonitors.Text += screenID + "; ";
            }
            if (Settings.Default.use_background_image) {
                mainWindow.txt_selectedBackground.Text = Settings.Default.background_image;
            }
            else {
                mainWindow.txt_selectedBackground.Text = Settings.Default.background_color;
            }
            Close();
        }

        private void btn_identify_Click(object sender, RoutedEventArgs e) {
            if (showingIdentifier) {
                foreach(IdentifierWindow window in identifierWindows) {
                    window.Close();
                }
                identifierWindows.Clear();
                showingIdentifier = false;
            }
            else {
                foreach (Screen screen in connectedScreens) {
                    IdentifierWindow identifier = new IdentifierWindow(screen.DeviceName);
                    identifier.Left = screen.Bounds.Left;
                    identifier.Top = screen.Bounds.Top;
                    identifier.Width = screen.Bounds.Width;
                    identifier.Height = screen.Bounds.Height;
                    identifierWindows.Add(identifier);
                    identifier.Show();
                    showingIdentifier = true;
                }
            }
        }

        private void btn_configure_Click(object sender, RoutedEventArgs e) {
            if (rad_image.IsChecked == true) {
                //open file chooser
                OpenFileDialog fileChooser = new OpenFileDialog();
                fileChooser.Title = "Select an image file...";
                fileChooser.Filter = "Image files(*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                fileChooser.ShowDialog();
                string result = fileChooser.FileName;
                Settings.Default.background_image = result;
                Settings.Default.use_background_image = true;
            }
            else if (rad_solidColor.IsChecked == true) {
                //open color chooser
                Settings.Default.use_background_image = false;
                ColorPickerWindow window = new ColorPickerWindow();
                window.ToggleSimpleAdvancedView();
                window.Topmost = true;
                Topmost = false;
                ColorPickerWindow.ShowDialog(out Color output);
                SolidColorBrush brush = new SolidColorBrush(output);
                Settings.Default.background_color = brush.ToString();
                Topmost = true;
            }
        }

        private void lstBox_monitors_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            String selectedMonitorJson = "{";
            int counter = 1;
            foreach(String screenID in lstBox_monitors.SelectedItems) {
                if (counter < lstBox_monitors.SelectedItems.Count) {
                    selectedMonitorJson += screenID + ", ";
                    counter++;
                }
                else {
                    selectedMonitorJson += screenID;
                }
            }
            selectedMonitorJson += "}";
            Settings.Default.selected_monitors = selectedMonitorJson;
        }

        private void btn_configFont_Click(object sender, RoutedEventArgs e) {
            FontConfiguration config = new FontConfiguration(this);
            config.Show();
        }
    }
}
