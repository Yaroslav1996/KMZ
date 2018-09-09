using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace KMZ.Controls
{
    /// <summary>
    /// Interaction logic for EditPointControl.xaml
    /// </summary>
    public partial class EditPointControl : UserControl
    {
        public EditPointControl(SharpKml.Base.Vector coordinates, int pointNumber)
        {
            InitializeComponent();
            Coordinates = coordinates;
            CurrLat.Text = Coordinates.Latitude.ToString();
            CurrLong.Text = Coordinates.Longitude.ToString();
            Point.Text = pointNumber.ToString();
            PreviewTextInput += NumberValidation;
        }

        public SharpKml.Base.Vector Coordinates { get; set; }

        private void OnConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Coordinates.Latitude = Double.Parse(NewLat.Text);
                CurrLat.Text = Coordinates.Latitude.ToString();
            }
            catch
            {

            }

            try
            {
                Coordinates.Longitude = Double.Parse(NewLong.Text);
                CurrLong.Text = Coordinates.Longitude.ToString();
            }
            catch
            {

            }
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex("[^0-9]+");
            e.Handled = reg.IsMatch(e.Text);
        }
    }
}
