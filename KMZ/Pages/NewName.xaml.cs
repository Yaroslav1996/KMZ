using SharpKml.Dom;
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
using SharpKml.Engine;

namespace KMZ.Pages
{
    /// <summary>
    /// Interaction logic for NewName.xaml
    /// </summary>
    public partial class NewName : Window
    {
        public NewName(MainWindow window)
        {
            InitializeComponent();
            wind = window;
        }

        MainWindow wind;

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            ((Kml)wind.ChosenFile.Root).Feature.Name = this.NewNameBox.Text;
            foreach(FileButton<KmlFile> i in wind.Stack.Children)
            {
                if (i.IsClicked)
                {
                    if (this.NewNameBox.Text.Contains(".kml"))
                        i.Content = this.NewNameBox.Text;

                    else
                    {
                        i.Content = this.NewNameBox.Text + ".kml";
                    }
                }
            }
            this.Close();
        }
    }
}
