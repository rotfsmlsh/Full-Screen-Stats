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
using WpfScreenHelper;

namespace FullScreenStats {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            //default selected monitor to primary display
            if (Settings.Default.selected_monitors.Length == 0) {
                Settings.Default.selected_monitors = "{" + Screen.PrimaryScreen.DeviceName + "}";
            }
            //populate check boxes from settings
            populateMonitorSettingsText();
            populateSettingsChecklist();
        }

        private void previewButton_Click(object sender, RoutedEventArgs e) {
            FSSWindow.destroyForms();
            if (Settings.Default.use_background_image == true) {
                //set the background as an image
                new FSSWindow(Settings.Default.selected_monitors, Settings.Default.use_background_image, Settings.Default.background_image);
            }
            else {
                new FSSWindow(Settings.Default.selected_monitors, Settings.Default.use_background_image, Settings.Default.background_color);
            }
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e) {
            Close();
            Settings.Default.Save();
            Environment.Exit(0);
        }

        private void btn_monitorSelection_Click(object sender, RoutedEventArgs e) {
            MonitorSettings settings = new MonitorSettings(this);
            settings.Show();
        }

        private void chk_systemTime_Checked(object sender, RoutedEventArgs e) {
            //Open dialog with chooser for format
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
            //GPU only for now, need to sort out CPU
            Settings.Default.show_temps = true;
        }

        private void chk_systemTemps_Unchecked(object sender, RoutedEventArgs e) {
            Settings.Default.show_temps = false;
        }

        //private void chk_music_Unchecked(object sender, RoutedEventArgs e) {
        //    Settings.Default.show_media = false;
        //}

        //private void chk_music_Checked(object sender, RoutedEventArgs e) {
        //    Settings.Default.show_media = true;
        //}


        private void populateSettingsChecklist() {
            //chk_music.IsChecked = Settings.Default.show_media;
            chk_systemTemps.IsChecked = Settings.Default.show_temps;
            chk_network.IsChecked = Settings.Default.show_networkStats;
            chk_systemTime.IsChecked = Settings.Default.show_systemTime;
        }

        private void populateMonitorSettingsText() {
            txt_selectedMonitors.Text = Settings.Default.selected_monitors;
            if (Settings.Default.use_background_image) {
                txt_selectedBackground.Text = Settings.Default.background_image;
            }
            else {
                txt_selectedBackground.Text = Settings.Default.background_color;
            }
        }
    }
}
