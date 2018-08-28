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
using System.Windows.Shapes;
using KMZ.Classes;

namespace KMZ.Pages
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(MainWindow wind)
        {
            InitializeComponent();
            window = wind;
            #region SpareCopy Init
            if (window.Setts.SpareCopy == Setting.Yes)
            {
                Copy_Yes.IsChecked = true;
            }
            else if(window.Setts.SpareCopy == Setting.No)
            {
                Copy_No.IsChecked = true;
            }
            else
            {
                Copy_Maybe.IsChecked = true;
            }
            #endregion
        }

        MainWindow window;

        private void OnCopy_Yes_Checked(object sender, RoutedEventArgs e)
        {
            window.Setts.SpareCopy = Setting.Yes;
            window.SaveSettings();
        }

        private void OnCopy_No_Checked(object sender, RoutedEventArgs e)
        {
            window.Setts.SpareCopy = Setting.No;
            window.SaveSettings();
        }

        private void OnCopy_Maybe_Checked(object sender, RoutedEventArgs e)
        {
            window.Setts.SpareCopy = Setting.Maybe;
            window.SaveSettings();
        }
    }
}
