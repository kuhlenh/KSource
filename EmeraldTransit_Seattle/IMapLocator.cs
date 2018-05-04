using System.Threading.Tasks;
using Amazon.Lambda.Core;
//using BusInfo;

namespace EmeraldTransit_Seattle {
	public interface IMapLocator
    {
        Task<(string, string)> GoogleMapGeocodeLocation(ILambdaLogger log, string address);
    }


}
