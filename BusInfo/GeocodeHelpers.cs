using BusInfo;
using BusInfoHelpers;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BusInfo
{
    class GeocodeHelpers
    {
        private static string latitude = "47.639905";
        private static string longitude = "-122.125485";

        public static (string lat, string lon) GetDefaultLocation() {
            return (latitude, longitude);
        }
        public static string PrettyPrintGeocode(string lat, string lon)
        {
            var latitude = double.Parse(lat);
            var longitude = double.Parse(lon);
            return $"\nCurrent location: ({latitude},{longitude})\n";
        }

        // Checks if given latitude and longitude are valid entries
        public static void ValidateLatLon(string lat, string lon) {
            if (lat.Length > 0 && lon.Length > 0) {
                double latDouble = Double.Parse(lat);
                var lonDouble = Double.Parse(lon);
                if (!(latDouble >= -90) || !(latDouble <= 90) || !(lonDouble >= -180) || !(lonDouble <= 180))
                    throw new ArgumentException("Exceeds boundaries. Not a valid latitude or longitude.");
            } else {
                throw new ArgumentException("You are missing latitude and longitude.");
            }
        }
    }
}
