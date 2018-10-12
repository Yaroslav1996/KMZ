using SharpKml.Engine;

namespace KMZ.Classes
{
    public class Profile
    {
        public Profile()
        {
        }

        public Profile(Section section, KmlFile kmlFile)
        {
            ProfSection = section;
            ProfKmlFile = kmlFile;
        }

        public Section ProfSection { get; set; }
        public KmlFile ProfKmlFile { get; set; }
    }
}