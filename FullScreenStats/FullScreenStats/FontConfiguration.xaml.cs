using FullScreenStats.Properties;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace FullScreenStats {
    /// <summary>
    /// Interaction logic for FontConfiguration.xaml
    /// </summary>
    public partial class FontConfiguration : Window {
        MonitorSettings settingsWindow;
        public FontConfiguration(MonitorSettings settingsWindow) {
            this.settingsWindow = settingsWindow;
            InitializeComponent();
            //populate fonts
            populateFontListBox();
            //update selected rad button
            updateStyleRadios();
        }

        private void populateFontListBox() {
            lst_fontSelection.ItemsSource = Fonts.SystemFontFamilies;
        }

        private void btn_close_Click(object sender, RoutedEventArgs e) {
            if (lst_fontSelection.SelectedIndex != -1) {
                Settings.Default.global_font = lst_fontSelection.SelectedItem.ToString();
                settingsWindow.txt_fontStyle.Text = lst_fontSelection.SelectedItem.ToString();
            }
            Settings.Default.global_font_size = (int)slide_fontSize.Value;
            Settings.Default.global_isBold = (bool)rad_bold.IsChecked || (bool)rad_both.IsChecked;
            Settings.Default.global_isItalic = (bool)rad_italic.IsChecked || (bool)rad_both.IsChecked;
            settingsWindow.txt_fontSize.Text = slide_fontSize.Value.ToString();
            Close();
        }

        private void updateStyleRadios() {
            if(Settings.Default.global_isBold && Settings.Default.global_isItalic) {
                rad_both.IsChecked = true;
            }
            else if (Settings.Default.global_isBold) {
                rad_bold.IsChecked = true;
            }
            else if (Settings.Default.global_isItalic) {
                rad_italic.IsChecked = true;
            }
            else {
                rad_none.IsChecked = true;
            }
        }
    }
}
