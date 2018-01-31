using System.Threading.Tasks;

namespace BusInfo
{
    public interface ITimeZoneConverter
    {
        Task<string> GetTimeZoneJsonFromLatLonAsync(string lat, string lon);
    }
}
