using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMZ.Classes
{
    public class Settings
    {
        public Settings()
        {
        }

        public Setting SpareCopy { get; set; }
    }

    public enum Setting
    {
        Yes = 1,
        No = -1,
        Maybe = 0 
    }
}
