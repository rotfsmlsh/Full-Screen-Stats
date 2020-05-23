using FullScreenStats.Properties;
using NvAPIWrapper.Display;
using NvAPIWrapper.DRS.SettingValues;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.Native.Interfaces.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Background = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(color));
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

            //FPS display
            if (Settings.Default.show_fps) {
                configAndShowFPS();
            }
            else {
                lbl_fps.Visibility = Visibility.Hidden;
            }

            //Media control/info
            if (Settings.Default.show_media) {
                lbl_media.Visibility = Visibility.Visible;
            }
            else {
                lbl_media.Visibility = Visibility.Hidden;
            }

            //Network stats
            if (Settings.Default.show_networkStats) {
                lbl_networkStats.Visibility = Visibility.Visible;
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
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                lbl_time.Content = DateTime.Now.ToString("dd MMMM yyyy\nHH:mm:ss");
            }, Dispatcher);
            lbl_time.Visibility = Visibility.Visible;
        }

        private void configAndShowFPS() {
            Display primaryDisplay = Display.GetDisplays()[0];          
            PathTargetInfo targetInfo = new PathTargetInfo(primaryDisplay.DisplayDevice);

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                
            }, Dispatcher);

            
            

            //IClockFrequencies freqs = primaryDisplay.LogicalGPU.CorrespondingPhysicalGPUs[0].CurrentClockFrequencies;
            //uint graphicsClockFreq = freqs.GraphicsClock.Frequency;
            //uint videoDecodeClockFreq = freqs.VideoDecodingClock.Frequency;
            //uint memoryClockFreq = freqs.MemoryClock.Frequency;
            //uint processorClockFreq = freqs.ProcessorClock.Frequency;

            //lbl_fps.Content = graphicsClockFreq + "\n" + videoDecodeClockFreq + "\n" + memoryClockFreq + "\n" + processorClockFreq;
            lbl_fps.Visibility = Visibility.Visible;
        }

        private void configAndShowTemps() {
            Display primaryDisplay = Display.GetDisplays()[0];
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate {
                GPUThermalSensor sensor = primaryDisplay.PhysicalGPUs[0].ThermalInformation.ThermalSensors.First();
                lbl_systemTemps.Content = "GPU Temp: " + sensor.CurrentTemperature + "\n";
                
            }, Dispatcher);
            lbl_systemTemps.Visibility = Visibility.Visible;
        }
    }
}
