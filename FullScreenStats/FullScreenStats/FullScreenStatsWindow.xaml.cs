using FullScreenStats.Properties;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;
using System.Xml.Linq;
using WpfScreenHelper;

namespace FullScreenStats {
    /// <summary>
    /// Interaction logic for FSSWindow.xaml
    /// </summary>
    public partial class FSSWindow : Window {
        static List<FSSWindow> openWindows = new List<FSSWindow>();
        System.Windows.Media.FontFamily family;
        

        public FSSWindow(String selectedMonitors, bool useImage, String color) {
            foreach(Screen screen in Screen.AllScreens) {
                if (selectedMonitors.Contains(screen.DeviceName)) {
                    openWindows.Add(new FSSWindow(screen, color));
                }
            }
        }

        private FSSWindow(Screen displayMonitor, String background) {
            family = new System.Windows.Media.FontFamily(Settings.Default.global_font);
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

        public void setColorFromString(string color) {
            if (color.Length > 0) {
                Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
            }
        }

        public void setColor(System.Windows.Media.Color selectedColor) {
            Background = new SolidColorBrush(selectedColor);
        }

        private void enableSettings() {
            //System clock
            if (Settings.Default.show_systemTime) {
                configAndShowTime();
            }
            else {
                lbl_time.Visibility = Visibility.Hidden;
            }

            //Media control/info
            if (Settings.Default.show_media) {
                configAndShowMediaInfo();
            }
            else {
                lbl_media.Visibility = Visibility.Hidden;
            }

            //Network stats
            if (Settings.Default.show_networkStats) {
                configAndShowNetworkStats();
            }
            else {
                lbl_networkStats.Visibility = Visibility.Hidden;
            }

            //System Temps
            if (Settings.Default.show_temps) {
                configAndShowTemps();
            }
            else {
                lbl_systemTemps.Visibility = Visibility.Hidden;
            }
        }

        public static void destroyForms() {
            foreach(FSSWindow window in openWindows) {
                window.Close();
            }
        }

        private void btn_closeAll_Click(object sender, RoutedEventArgs e) {
            destroyForms();
        }

        private void configAndShowTime() {
            lbl_time.FontFamily = family;
            lbl_time.FontSize = Settings.Default.global_font_size;
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                lbl_time.Content = DateTime.Now.ToString("dd MMMM yyyy\nHH:mm:ss");
            }, Dispatcher);
            lbl_time.Visibility = Visibility.Visible;
        }

        private void configAndShowTemps() {
            lbl_systemTemps.FontFamily = family;
            lbl_systemTemps.FontSize = Settings.Default.global_font_size;
            Display primaryDisplay = Display.GetDisplays()[0];
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                GPUThermalSensor sensor = primaryDisplay.PhysicalGPUs[0].ThermalInformation.ThermalSensors.First();
                lbl_systemTemps.Content = "GPU Temp: " + sensor.CurrentTemperature + "\n";
            }, Dispatcher);
            lbl_systemTemps.Visibility = Visibility.Visible;
        }

        private void configAndShowNetworkStats() {
            lbl_networkStats.FontFamily = family;
            lbl_networkStats.FontSize = Settings.Default.global_font_size;
            if (!NetworkInterface.GetIsNetworkAvailable()) {
                lbl_networkStats.Content = "Network information not available.";
                lbl_networkStats.Visibility = Visibility.Visible;
            }

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            float previousMBSent = 0f;
            float previousMBReceived = 0f;
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                string formattedNetworkStats = "";
                foreach(NetworkInterface netInterface in interfaces) {
                    if (netInterface.GetIPv4Statistics().BytesReceived > 0) {
                        float mbSent = netInterface.GetIPv4Statistics().BytesSent / 1e+6f;
                        float mbRecieved = netInterface.GetIPv4Statistics().BytesReceived / 1e+6f;
                        formattedNetworkStats += "Network Interface: " + netInterface.Name;
                        formattedNetworkStats += "\n  MB Sent      : " + netInterface.GetIPv4Statistics().BytesSent / 1e+6f;
                        formattedNetworkStats += "\n  Send Rate    : " + String.Format("{0:0.00}", (mbSent - previousMBSent)) + "MB/s";
                        formattedNetworkStats += "\n  MB Received  : " + netInterface.GetIPv4Statistics().BytesReceived /1e+6f;
                        formattedNetworkStats += "\n  Recieve Rate : " + String.Format("{0:0.00}", (mbRecieved - previousMBReceived)) + "MB/s";
                        formattedNetworkStats += "\n";
                        previousMBReceived = mbRecieved;
                        previousMBSent = mbSent;
                    }
                    lbl_networkStats.Content = formattedNetworkStats;
                }
            }, Dispatcher);

            lbl_networkStats.Visibility = Visibility.Visible;
        }

        private void configAndShowMediaInfo() {
            lbl_media.Visibility = Visibility.Visible;
        }
    }
}
