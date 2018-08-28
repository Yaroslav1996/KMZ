using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using SharpKml.Dom;
using SharpKml.Engine;

namespace KMZ.Classes
{
    public class KMLButton : Button
    {
        public KMLButton() { }

        public KMLButton(KmlFile input)
        {
            File = input;
        }

        public KmlFile File { get; set; }
        public bool IsClicked { get; set; }
    }
}
