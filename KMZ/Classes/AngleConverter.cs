using System;

namespace KMZ.Classes
{
    internal static class AngleConverter
    {
        public static double ConvertDegreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double ConvertRadiansToDegrees(double angle)
        {
            return 180.0 * angle / Math.PI;
        }
    }
}