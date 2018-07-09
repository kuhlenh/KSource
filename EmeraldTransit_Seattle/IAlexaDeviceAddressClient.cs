using System.Threading.Tasks;

namespace EmeraldTransit_Seattle
{
    public interface IAlexaDeviceAddressClient
    {
        string ApiEndpoint { get; set; }
        string DeviceId { get; set; }
        string ConsentToken { get; set; }
        Task<string> GetFullAddressAsync(ILogger logger);
    }
}
