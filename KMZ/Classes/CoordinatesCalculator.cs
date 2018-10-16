using System;
using Coord = SharpKml.Base;

namespace KMZ.Classes
{
    public static class CoordinatesCalculator
    {
        public static readonly int EarthsRadius = 6371;

        /// <summary>
        /// Calculates distance between two coordinates
        /// </summary>
        /// <param name="sLatitude">First point latitude in degrees</param>
        /// <param name="sLongitude">First point longitude in degrees</param>
        /// <param name="eLatitude">Second point latitude in degrees</param>
        /// <param name="eLongitude">Second point longitude in degrees</param>
        /// <returns>Distance in kilometers</returns>
        public static double CalculateDistance(double sLatitude, double sLongitude, double eLatitude, double eLongitude)
        {
            var radiansOverDegrees = (Math.PI / 180.0);

            var sLatitudeRadians = sLatitude * radiansOverDegrees;
            var sLongitudeRadians = sLongitude * radiansOverDegrees;
            var eLatitudeRadians = eLatitude * radiansOverDegrees;
            var eLongitudeRadians = eLongitude * radiansOverDegrees;

            var dLongitude = eLongitudeRadians - sLongitudeRadians;
            var dLatitude = eLatitudeRadians - sLatitudeRadians;

            var result1 = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                          Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) *
                          Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);
            
            var result2 = EarthsRadius * 2.0 * Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));

            return result2;
        }

        public static double CalculateBearing(Coord.Vector position1, Coord.Vector position2)
        {
            var lat1 = AngleConverter.ConvertDegreesToRadians(position1.Latitude);
            var lat2 = AngleConverter.ConvertDegreesToRadians(position2.Latitude);
            var long1 = AngleConverter.ConvertDegreesToRadians(position2.Longitude);
            var long2 = AngleConverter.ConvertDegreesToRadians(position1.Longitude);
            var dLon = long1 - long2;

            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            var brng = Math.Atan2(y, x);

            return (AngleConverter.ConvertRadiansToDegrees(brng) + 360) % 360;
        }

        public static Coord.Vector CalculatePoint(Coord.Vector start, double distance, double bearing)
        {
            double dist = distance / EarthsRadius;

            double lat = Math.Asin(Math.Sin(start.Latitude) * Math.Cos(distance) + Math.Cos(start.Latitude) * Math.Sin(dist) * Math.Cos(bearing));
            double dlon = Math.Atan2(Math.Sin(bearing) * Math.Sin(dist) * Math.Cos(start.Latitude), Math.Cos(dist) - Math.Sin(start.Latitude) * Math.Sin(lat));
            double lon = (start.Longitude - dlon + Math.PI) % 2 * Math.PI - Math.PI;

            return new Coord.Vector(lat, lon);
        }
    }
}
