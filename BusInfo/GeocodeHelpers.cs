using System;

namespace BusInfo
{
    public class GeocodeHelpers
    {
        private static readonly string latitude = "47.639905";
        private static readonly string longitude = "-122.125485";

        public static (string lat, string lon) GetDefaultLocation()
        {
            return (latitude, longitude);
        }
        public static string PrettyPrintGeocode(string lat, string lon)
        {
            var latitude = double.Parse(lat);
            var longitude = double.Parse(lon);
            return $"\nCurrent location: ({latitude},{longitude})\n";
        }

        // Checks if given latitude and longitude are valid entries
        public static void ValidateLatLon(string lat, string lon)
        {
            if (lat == null)
            {
                throw new ArgumentNullException(nameof(lat));
            }

            if (lon == null)
            {
                throw new ArgumentNullException(nameof(lon));
            }

            if (lat.Length == 0 || lon.Length == 0)
            {
                throw new ArgumentException("You are missing latitude and longitude.");
            }
            else
            {
                var latDouble = double.Parse(lat);
                var lonDouble = double.Parse(lon);
                if (!(latDouble >= -90) || !(latDouble <= 90) || !(lonDouble >= -180) || !(lonDouble <= 180))
                {
                    throw new ArgumentException("Exceeds boundaries. Not a valid latitude or longitude.");
                }
            }
        }

        internal static double CalculateDistanceFormula(string lat1, string lon1, double lat2, double lon2)
        {
            var distance = Math.Sqrt(Math.Pow(double.Parse(lat1) - lat2, 2) + Math.Pow(double.Parse(lon1) - lon2, 2));
            return distance;
        }
    }
}
