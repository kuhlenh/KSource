using System.Threading.Tasks;

namespace BusInfo
{
    public interface IBusLocator
    {
        Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon);
        Task<string> GetJsonForArrivals(string stopId);
    }
}
