using System;

namespace BusInfo
{
    public class GeocodeHelpers
    {
        // Checks if given latitude and longitude are valid entries
        public static void ValidateLatLon(string lat, string lon)
        {
            if (lat.Length > 0 && lon.Length > 0)
            {
                double latDouble = double.Parse(lat);
                double lonDouble = double.Parse(lon);
                if (!(latDouble >= -90) || !(latDouble <= 90) || !(lonDouble >= -180) || !(lonDouble <= 180))
                    throw new ArgumentException("Exceeds boundaries. Not a valid latitude or longitude.");
            }
            else
            {
                throw new ArgumentException("You are missing latitude and longitude.");
            }
        }

        public static string PrettyPrintGeocode(string lat, string lon)
        {
            double latitude = Math.Round(Double.Parse(lat), 3);
            var longitude = Math.Round(Double.Parse(lon), 3);
            return $"\nCurrent location: ({latitude},{longitude})\n";
        }

        public static (string lat, string lon) GetDefaultLocation()
        {
            return ("47.639905", "-122.125485");
        }
    }
}
