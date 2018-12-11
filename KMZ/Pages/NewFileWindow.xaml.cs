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
        public NewFileWindow()
        {
            InitializeComponent();
            CurrentPointNumber = 0;
            NewFileName.Focus();
            NewFileName.SelectAll();
            CoordsList = new List<SharpKml.Base.Vector>();
            NewKmlFile = new Kml();
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
        public List<SharpKml.Base.Vector> CoordsList { get; set; }
        public Kml NewKmlFile { get; set; }
        public KmlFile OutFile;

        private void CloseIt()
        {
            LineString line = new LineString();
            line.Coordinates = new CoordinateCollection(CoordsList);
            Placemark placemark = new Placemark();
            placemark.Name = this.NewFileName.Text + ".kml";
            placemark.Geometry = line;
            NewKmlFile.Feature = placemark;

            OutFile = KmlFile.Create(NewKmlFile, true);


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
