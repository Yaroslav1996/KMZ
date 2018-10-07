using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using KMZ.Classes;

namespace KMZ.Pages
{
    /// <summary>
    /// Interaction logic for SectionReadWindow.xaml
    /// </summary>
    public partial class SectionReadWindow : Window
    {
        public SectionReadWindow(Section section)
        {
            InitializeComponent();
            SectionName.Text = section.Name;
            SectionImage.Source = BitmapConversion.ToWpfBitmap(section.Image);
        }

       
    }
}
