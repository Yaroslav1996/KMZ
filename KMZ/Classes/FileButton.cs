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
    public class FileButton<T> : Button
    {
        public FileButton() { }

        public FileButton(T input)
        {
            File = input;
        }

        public T File { get; set; }
        public bool IsClicked { get; set; }
    }
}
