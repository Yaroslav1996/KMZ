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
        public NewName(StringPackage p)
        {
            InitializeComponent();
            Namee = p;
        }

        StringPackage Namee;

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            Namee.Content = NewNameBox.Text;
            
            this.Close();
        }
    }
}
