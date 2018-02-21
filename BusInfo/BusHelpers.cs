using System;
using System.Text.RegularExpressions;

namespace BusInfo
{
    public static class BusHelpers
    {
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

            if (lat.Length > 0 && lon.Length > 0)
            {
                double latDouble = double.Parse(lat);
                double lonDouble = double.Parse(lon);
                if (!(latDouble >= -90) || !(latDouble <= 90) || !(lonDouble >= -180) || !(lonDouble <= 180))
                    throw new ArgumentException("Not a valid latitude or longitude.");
            }
            else
            {
                throw new ArgumentException("Not a valid latitude or longitude.");
            }
        }

        // Removes the identifier from route name, e.g., ###E for Express routes
        public static string CleanRouteName(string routeShortName) => Regex.Replace(routeShortName, "[^0-9]", "");

        // Uses distance formula to find distance between two points
        public static double CalculateDistance(string lat1, string lon1, double lat2, double lon2)
        {
            return Math.Sqrt(Math.Pow(double.Parse(lat1) - lat2, 2)
                   + Math.Pow(double.Parse(lon1) - lon2, 2));
        }
    }
}
