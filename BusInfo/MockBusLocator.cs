using System.IO;
using System.Threading.Tasks;

namespace BusInfo
{
    public class MockBusLocator : IBusLocator
    {
        private static string LoadJson(string file)
        {
            return File.ReadAllText(file);
        }

        public Task<string> GetJsonForArrivals(string stopId)
        {
            var json = LoadJson(@"C:\Users\kaseyu\Source\Repos\FunctionApp1\UnitTestProject\Arrivals.json");
            return Task.FromResult(json);
        }

        public Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon)
        {
            var json = LoadJson(@"C:\Users\kaseyu\Source\Repos\FunctionApp1\UnitTestProject\StopsForLoc.json");
            return Task.FromResult(json);
        }
    }
}
