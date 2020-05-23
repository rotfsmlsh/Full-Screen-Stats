using FullScreenStats.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for FSSWindow.xaml
    /// </summary>
    public partial class FSSWindow : Window {
        static List<FSSWindow> openWindows = new List<FSSWindow>();
        
        public FSSWindow(String selectedMonitors, bool useImage, String color) {
            foreach(Screen screen in Screen.AllScreens) {
                if (selectedMonitors.Contains(screen.DeviceName)) {
                    openWindows.Add(new FSSWindow(screen, color));
                }
            }
        }

        private FSSWindow(Screen displayMonitor, String background) {
            InitializeComponent();
            if (Settings.Default.use_background_image) {
                Uri path = new Uri(background);
                Background = new ImageBrush(new BitmapImage(path));
            }
            else {
                setColorFromString(background);
            }
            Top = displayMonitor.Bounds.Top;
            Left = displayMonitor.Bounds.Left;
            Width = displayMonitor.Bounds.Width;
            Height = displayMonitor.Bounds.Height;
            Title = displayMonitor.DeviceName;
            enableSettings();
            Show();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        public void setColorFromString(String color) {
            if (color.Length > 0) {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
        }

        public void setColor(Color selectedColor) {
            Background = new SolidColorBrush(selectedColor);
        }

        private void enableSettings() {
            lbl_time.Visibility = Settings.Default.show_systemTime == true ? Visibility.Visible : Visibility.Hidden;
            lbl_fps.Visibility = Settings.Default.show_fps == true ? Visibility.Visible : Visibility.Hidden;
            lbl_media.Visibility = Settings.Default.show_media == true ? Visibility.Visible : Visibility.Hidden;
            lbl_networkStats.Visibility = Settings.Default.show_networkStats == true ? Visibility.Visible : Visibility.Hidden;
            lbl_systemTemps.Visibility = Settings.Default.show_temps == true ? Visibility.Visible : Visibility.Hidden;
        }

        public static void destroyForms() {
            foreach(FSSWindow window in openWindows) {
                window.Close();
            }
        }

        private void btn_closeAll_Click(object sender, RoutedEventArgs e) {
            destroyForms();
        }

        private void run_systemTime() {
            
        }

    }
}
