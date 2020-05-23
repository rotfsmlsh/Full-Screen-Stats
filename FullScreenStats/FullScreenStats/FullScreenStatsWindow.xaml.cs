﻿using System;
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
using System.Windows.Shapes;

namespace FullScreenStats {
    /// <summary>
    /// Interaction logic for FSSWindow.xaml
    /// </summary>
    public partial class FSSWindow : Window {
        public FSSWindow() {
            InitializeComponent();
        }

        public FSSWindow(String selectedMonitors, Color color) {
            setColor(color);
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        public void setColor(Color selectedColor) {
            Background = new SolidColorBrush(selectedColor);
        }
    }
}
