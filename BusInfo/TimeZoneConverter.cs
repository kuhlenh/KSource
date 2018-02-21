using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusInfo
{
    public class TimeZoneConverter : ITimeZoneConverter
    {
        HttpClient http = new HttpClient();
        private const string Key = "AIzaSyAKLwQo-xS-a7HChxZDjBvxHxyo0vCj8RE";

        public async Task<string> GetTimeZoneJsonFromLatLonAsync(string lat, string lon)
        {
            var unix = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var url = $"https://maps.googleapis.com/maps/api/timezone/json?location={lat},{lon}&timestamp={unix}&sensor=false";
            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return "";
            }
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }
    }
}
