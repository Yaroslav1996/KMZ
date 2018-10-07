using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KMZ.Classes
{
    public class Section
    {
        public Section()
        {
        }

        public Section(string n, Bitmap im)
        {
            Name = n;
            Image = im;
        }

        public String Name { get; set; }

        public Bitmap Image { get; set; }
    }
}
