using FullScreenStats.Properties;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FullScreenStats {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void previewButton_Click(object sender, RoutedEventArgs e) {
            FSSWindow window = new FSSWindow();
            if (Settings.Default.use_background_image == true) {
                //set the background as an image
            }
            else {
            //    Color backgroundColor = (Color)ColorConverter.ConvertFromString(Settings.Default.background_color);
              //  window.Background = new SolidColorBrush(backgroundColor);
            }
            window.setColor((Color)ColorConverter.ConvertFromString(Settings.Default.background_color));
            window.Show();
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e) {
            Close();
            Environment.Exit(0);
        }

        private void combo_color_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }     

        private void btn_monitorSelection_Click(object sender, RoutedEventArgs e) {
            MonitorSettings settings = new MonitorSettings();
            settings.Show();
        }

        private void chk_systemTime_Checked(object sender, RoutedEventArgs e) {
            Settings.Default.show_systemTime = true;
        }
        private void chk_systemTime_Unchecked(object sender, RoutedEventArgs e) {
            Settings.Default.show_systemTime = false;
        }

        private void chk_network_Checked(object sender, RoutedEventArgs e) {
            Settings.Default.show_networkStats = true;
        }

        private void chk_network_Unchecked(object sender, RoutedEventArgs e) {
            Settings.Default.show_networkStats = false;
        }

        private void chk_systemTemps_Checked(object sender, RoutedEventArgs e) {
            Settings.Default.show_temps = true;
        }

        private void chk_systemTemps_Unchecked(object sender, RoutedEventArgs e) {
            Settings.Default.show_temps = false;
        }

        private void chk_music_Unchecked(object sender, RoutedEventArgs e) {
            Settings.Default.show_media = false;
        }

        private void chk_music_Checked(object sender, RoutedEventArgs e) {
            Settings.Default.show_media = true;
        }
        
        private void chk_FPS_Checked(object sender, RoutedEventArgs e) {
            Settings.Default.show_fps = true;
        }
        
        private void chk_FPS_Unchecked(object sender, RoutedEventArgs e) {
            Settings.Default.show_fps = false;
        }
    }
}
