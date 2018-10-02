using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZ.Classes
{
    public class Section
    {
        public Section(string n, Image im)
        {
            Name = n;
            Sec = im;
        }

        public String Name { get; set; }

        public Image Sec { get; set; }
    }
}
