using System.Net.Http;
using System.Threading.Tasks;

namespace BusInfo
{
    public class BusLocator : IBusLocator
    {
        HttpClient http = new HttpClient();
        private const string Key = "b5e9eb4f-d3f1-4d68-9ccf-4272385feb06";

        public async Task<string> GetJsonForArrivals(string stopId)
        {
            var url = $"http://api.pugetsound.onebusaway.org/api/where/arrivals-and-departures-for-stop/{stopId}.json?key={Key}";
            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return "";
            }
            var json = await response.Content.ReadAsStringAsync();
            return json;
        }

        public async Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon)
        {
            var url = $"http://api.pugetsound.onebusaway.org/api/where/stops-for-location.json?key={Key}&lat={lat}&lon={lon}&radius=1800&maxCount=50";
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
