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
            foreach (Screen screen in Screen.AllScreens) {
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

            Show();

            Title = displayMonitor.DeviceName;
            enableSettings();
            Show();
        }

        private void btn_closeThis_Click(object sender, RoutedEventArgs e) {
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
            foreach (FSSWindow window in openWindows) {
                window.Close();
            }
        }

        private void btn_closeAll_Click(object sender, RoutedEventArgs e) {
            destroyForms();
        }

        private void configAndShowTime() {
            configFont(lbl_time);
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                lbl_time.Content = DateTime.Now.ToString("dd MMMM yyyy\nHH:mm:ss");
            }, Dispatcher);
            lbl_time.Visibility = Visibility.Visible;
        }

        private void configAndShowTemps() {
            configFont(lbl_systemTemps);
            Display primaryDisplay = Display.GetDisplays()[0];
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                GPUThermalSensor sensor = primaryDisplay.PhysicalGPUs[0].ThermalInformation.ThermalSensors.First();
                lbl_systemTemps.Content = "GPU Temp: " + sensor.CurrentTemperature + "\n";
            }, Dispatcher);
            lbl_systemTemps.Visibility = Visibility.Visible;
        }

        private void configAndShowNetworkStats() {
            configFont(lbl_networkStats);
            if (!NetworkInterface.GetIsNetworkAvailable()) {
                lbl_networkStats.Content = "Network information not available.";
                lbl_networkStats.Visibility = Visibility.Visible;
            }

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            float previousMBSent = 0f;
            float previousMBReceived = 0f;
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                string formattedNetworkStats = "";
                foreach (NetworkInterface netInterface in interfaces) {
                    if (netInterface.GetIPv4Statistics().BytesReceived > 0) {
                        float mbSent = netInterface.GetIPv4Statistics().BytesSent / 1e+6f;
                        float mbRecieved = netInterface.GetIPv4Statistics().BytesReceived / 1e+6f;
                        formattedNetworkStats += "Network Interface: " + netInterface.Name;
                        formattedNetworkStats += "\n  MB Sent      : " + netInterface.GetIPv4Statistics().BytesSent / 1e+6f;
                        formattedNetworkStats += "\n  Send Rate    : " + String.Format("{0:0.00}", (mbSent - previousMBSent)) + "MB/s";
                        formattedNetworkStats += "\n  MB Received  : " + netInterface.GetIPv4Statistics().BytesReceived / 1e+6f;
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

        private void configFont(Label label) {
            label.FontFamily = family;
            label.FontSize = Settings.Default.global_font_size;
            if (Settings.Default.global_isBold && Settings.Default.global_isItalic) {
                label.FontWeight = FontWeight.FromOpenTypeWeight(700);
                label.FontStyle = FontStyles.Italic;
            }
            else if (Settings.Default.global_isBold) {
                label.FontWeight = FontWeight.FromOpenTypeWeight(700);
            }
            else if (Settings.Default.global_isItalic) {
                label.FontStyle = FontStyles.Italic;
            }
            else {
                label.FontWeight = FontWeight.FromOpenTypeWeight(400);
                label.FontStyle = FontStyles.Normal;
            }

            if (Settings.Default.use_background_image) {
                // add a background to the text box
                label.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
                label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            }
            else {
                label.Foreground = invertColor(Settings.Default.background_color);
            }
            
        }

        //shoutout to Jacob Saylor for his code snippet (https://jacobmsaylor.com/invert-a-color-c/)
        private static System.Windows.Media.Color HexToColor(string hexColor) {
            if (hexColor.IndexOf('#') != -1) {
                hexColor = hexColor.Replace("#", "");
            }
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            if (hexColor.Length == 8) {
                hexColor = hexColor.Substring(2);
            }

            if (hexColor.Length == 6) {
                //#RRGGBB
                red = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                green = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                blue = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            else if (hexColor.Length == 3) {
                //#RGB
                red = byte.Parse(hexColor[0].ToString() + hexColor[0].ToString());
                green = byte.Parse(hexColor[1].ToString() + hexColor[1].ToString());
                blue = byte.Parse(hexColor[2].ToString() + hexColor[2].ToString());
            }

            return System.Windows.Media.Color.FromRgb(red, green, blue);
        }

        private System.Windows.Media.Brush invertColor(string value) {
            if (value != null) {
                System.Windows.Media.Color ColorToConvert = HexToColor(value);
                System.Windows.Media.Color invertedColor = System.Windows.Media.Color.FromRgb((byte)~ColorToConvert.R, (byte)~ColorToConvert.G, (byte)~ColorToConvert.B);
                return new SolidColorBrush(invertedColor);
            }
            else {
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            }
        }
    }
}
