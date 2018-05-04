using System.Threading.Tasks;
using Amazon.Lambda.Core;
//using BusInfo;

namespace EmeraldTransit_Seattle {
	public class MockMapLocator : IMapLocator
    {
        public Task<(string, string)> GoogleMapGeocodeLocation(ILambdaLogger log, string address)
        {
            return Task.FromResult(("47.611959", "-122.332893"));
        }
    }


}
