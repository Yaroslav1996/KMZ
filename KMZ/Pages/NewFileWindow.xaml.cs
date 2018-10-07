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
using SharpKml.Dom;
using SharpKml.Base;
using SharpKml.Engine;
using KMZ.Controls;

namespace KMZ.Pages
{
    /// <summary>
    /// Interaction logic for NewFileWindow.xaml
    /// </summary>
    public partial class NewFileWindow : Window
    {
        public NewFileWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            CurrentPointNumber = 0;
            NewFileName.Focus();
            NewFileName.SelectAll();
            main = mainWindow;
            CoordsList = new List<SharpKml.Base.Vector>();
            Fajel = new Kml();
        }

        private void OnNewPointButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPointNumber++;
            SharpKml.Base.Vector coord = new SharpKml.Base.Vector(0, 0);
            CoordsList.Add(coord);
            EditPointControl pointControl = new EditPointControl(coord, CurrentPointNumber);
            this.Stack.Children.Add(pointControl);
        }

        public int CurrentPointNumber { get; set; }
        public MainWindow main { get; set; }
        public List<SharpKml.Base.Vector> CoordsList { get; set; }
        public Kml Fajel { get; set; }

        private void CloseIt()
        {
            LineString line = new LineString();
            line.Coordinates = new CoordinateCollection(CoordsList);
            Placemark placemark = new Placemark();
            placemark.Name = this.NewFileName.Text + ".kml";
            placemark.Geometry = line;
            Fajel.Feature = placemark;
           
            main.AddToList(KmlFile.Create(Fajel, true));

            main.IsEnabled = true;
            Close();
        }

        private void OnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewFileWindow_Closed(object sender, EventArgs e)
        {
            CloseIt();

        }
    }
}
