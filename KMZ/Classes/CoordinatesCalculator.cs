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
        public static double CalculateDistance(double elat1, double elon1, double elat2, double elon2) //works
        {
            var radiansOverDegrees = (Math.PI / 180.0);

            var sLatitudeRadians = elat1 * radiansOverDegrees;
            var sLongitudeRadians = elon1 * radiansOverDegrees;
            var eLatitudeRadians = elat2 * radiansOverDegrees;
            var eLongitudeRadians = elon2 * radiansOverDegrees;

            var dLongitude = eLongitudeRadians - sLongitudeRadians;
            var dLatitude = eLatitudeRadians - sLatitudeRadians;

            var result1 = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                          Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) *
                          Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            var result2 = EarthsRadius * 2.0 * Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));

            return result2;
        }

        public static double CalculateBearing(Coord.Vector position1, Coord.Vector position2) //works
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

        /// <summary>
        /// Calculates a point with a starting point, distance and azimuth
        /// </summary>
        /// <param name="start">Starting point in latitude and longitude</param>
        /// <param name="distance">Distance in kilometers</param>
        /// <param name="bearing">Azimuth in degrees</param>
        /// <returns></returns>
        public static Coord.Vector CalculatePoint(Coord.Vector start, double distance, double bearing)
        {
            double dista = distance / EarthsRadius;
            double tc = -AngleConverter.ConvertDegreesToRadians(bearing);
            double lat1 = AngleConverter.ConvertDegreesToRadians(start.Latitude);
            double lon1 = AngleConverter.ConvertDegreesToRadians(start.Longitude);

            double lat = Math.Asin(Math.Sin(lat1) * Math.Cos(dista) + Math.Cos(lat1) * Math.Sin(dista) * Math.Cos(tc));
            double dlon = Math.Atan2(Math.Sin(tc) * Math.Sin(dista) * Math.Cos(lat1), Math.Cos(dista) - Math.Sin(lat1) * Math.Sin(lat));
            double lon = ((lon1 - dlon + Math.PI) % (2 * Math.PI)) - Math.PI;

            lat = AngleConverter.ConvertRadiansToDegrees(lat);
            lon = AngleConverter.ConvertRadiansToDegrees(lon);

            return new Coord.Vector(lat, lon);
        }
    }
}