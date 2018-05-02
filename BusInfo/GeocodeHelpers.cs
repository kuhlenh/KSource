﻿using System;

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

        // Uses distance formula to find distance between two points
        // demo Pythia?
        //public static double CalculateDistance(string lat1, string lon1, double lat2, double lon2)
        //{
        //    return Math.Sqrt(Math.Pow(double.Parse(lat1) - lat2, 2)
        //           + Math.Pow(double.Parse(lon1) - lon2, 2));
        //}
    }
}
