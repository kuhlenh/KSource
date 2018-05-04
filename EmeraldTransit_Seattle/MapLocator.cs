using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
//using BusInfo;
using Newtonsoft.Json;

namespace EmeraldTransit_Seattle {
	public class MapLocator: IMapLocator
    {
        public MapLocator(HttpClient client)
        {
            this.client = client;
        }

        private HttpClient client;

        public async Task<(string, string)> GoogleMapGeocodeLocation(ILambdaLogger log, string address)
        {
            client.DefaultRequestHeaders.Clear();
            var key = "AIzaSyAKLwQo-xS-a7HChxZDjBvxHxyo0vCj8RE";
            var uriGoogle = new Uri($"https://maps.google.com/maps/api/geocode/json?key={key}&address={address}&sensor=false");
            var responseGeocode = await client.GetAsync(uriGoogle);
            if (!responseGeocode.IsSuccessStatusCode)
            {
                log.LogLine("Google Exception: " + responseGeocode.StatusCode);
                log.LogLine("Google Exception: " + responseGeocode.ReasonPhrase);

                throw new Exception("Google Geocode API failed.");
            }
            var json2 = await responseGeocode.Content.ReadAsStringAsync();
            var geocode = JsonConvert.DeserializeObject<Geocode>(json2);
            log.LogLine("\ngoogle: " + json2);
            var lat = geocode.results.FirstOrDefault().geometry.location.lat.ToString();
            log.LogLine("lat " + lat);
            var lng = geocode.results.FirstOrDefault().geometry.location.lng.ToString();
            log.LogLine("lng" + lng);
            return (lat, lng);
        }
    }


}
