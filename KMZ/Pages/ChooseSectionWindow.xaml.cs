using KMZ.Classes;
using System.Windows;

namespace KMZ.Pages
{
    /// <summary>
    /// Interaction logic for ChooseSectionWindow.xaml
    /// </summary>
    public partial class ChooseSectionWindow : Window
    {
        public ChooseSectionWindow(Profile profile)
        {
            InitializeComponent();
            Prof = profile;
        }

        public Profile Prof { get; set; }

        public void OnButtClick(object sender, RoutedEventArgs e)
        {
            FileButton<Section> butt = sender as FileButton<Section>;
            Prof.ProfSection = butt.File;
            this.Close();
        }
    }
}
