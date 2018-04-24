using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using BusInfo;
//using BusInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace EmeraldTransit_Seattle
{

    public class Function
    {
        HttpClient client = new HttpClient();
        //Func<T> GetResult<T>(T prototype) { return () => prototype; }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            //create a response to return
            SkillResponse response = new SkillResponse();
            response.Response = new ResponseBody();
            response.Response.ShouldEndSession = false;
            IOutputSpeech innerResponse = new PlainTextOutputSpeech() { Text = "testing" };
            var log = context.Logger;

            log.LogLine("\nStarting function handler...");

            Type requestType = input.GetRequestType();
            if (requestType == typeof(LaunchRequest))
            {
                log.LogLine($"\nDefault LaunchRequest made");
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = "Where would you like to go?";
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;
                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"\nAMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Stopping!";
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"\nAMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Stopping";
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"\nAMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Say the name of your bus route.";
                        break;
                    case "GetRouteIntent":
                        log.LogLine("\nStarting GetRouteIntent..." + "\n" + intentRequest.ToString());

                        innerResponse = new PlainTextOutputSpeech();
                        var consentToken = input.Context.System.ApiAccessToken;
                        log.LogLine("\nconsent token: " + consentToken);

                        if (string.IsNullOrEmpty(consentToken))
                        {
                            log.LogLine("\nUser did not give permission to access their location.");
                            var permissionCard = new AskForPermissionsConsentCard();
                            permissionCard.Permissions.Add(RequestedPermission.FullAddress);
                            var speech = new SsmlOutputSpeech();
                            speech.Ssml = "<speak>You need to enable permissions.</speak>";

                            // create and return the card response
                            response = ResponseBuilder.TellWithAskForPermissionsConsentCard(speech, permissionCard.Permissions);
                            return response;
                        }

                        var time = input.Request.Timestamp;
                        var deviceId = input.Context.System.Device.DeviceID;
                        var apiEndpoint = input.Context.System.ApiEndpoint;
                        var route = intentRequest.Intent.Slots["RouteName"].Value;
                        log.LogLine("\nTrying new version...");
                        // get the device's location to find the nearest bus stop
                        var alexaDeviceAddressClient = new AlexaDeviceAddressClient(apiEndpoint, deviceId, consentToken);
                        var address = await alexaDeviceAddressClient.GetFullAddressAsync(log);
                        log.LogLine("==================================");
                        log.LogLine("\nAddress: " + address);
                        log.LogLine("==================================");

                        // call into the Google Maps api to get geocode location
                        var location = await GoogleMapGeocodeLocation(log, address);

                        // call into the OBA api to get bus times
                        var sb = await GetBusResponse(route, location);
                        (innerResponse as PlainTextOutputSpeech).Text = sb;
                        log.LogLine("Final output == " + sb);
                        break;
                    default:
                        log.LogLine($"\nUnknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Say the name of your bus route";
                        break;
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            return response;
        }

        private static async Task<string> GetBusResponse(string route, (string, string) location)
        {
            MyStopInfo busInfo = new MyStopInfo(new BusLocator(), new TimeZoneConverter());
            var arrivalTimes = await busInfo.GetArrivalTimesForRouteName(route, location.Item1, location.Item2);
            StringBuilder sb = new StringBuilder();
            sb.Append($"The next {route} comes in ");
            foreach (var arrival in arrivalTimes)
            {
                sb.Append(arrival + " minutes,");
            }

            return sb.ToString();
        }

        private async Task<(string, string)> GoogleMapGeocodeLocation(ILambdaLogger log, string address)
        {
            client.DefaultRequestHeaders.Clear();
            var key = "AIzaSyAKLwQo-xS-a7HChxZDjBvxHxyo0vCj8RE";
            var uriGoogle = new Uri($"https://maps.google.com/maps/api/geocode/json?key={key}&address={address}&sensor=false");
            var responseGeocode = await client.GetAsync(uriGoogle);
            if (!responseGeocode.IsSuccessStatusCode)
            {
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

    class AlexaDeviceAddressClient
    {
        public string ApiEndpoint { get; set; }
        public string DeviceId { get; set; }
        public string ConsentToken { get; set; }
        HttpClient client = new HttpClient();

        public AlexaDeviceAddressClient(string apiEndpoint, string deviceId, string consentToken)
        {

            ApiEndpoint = Regex.Replace(apiEndpoint, @"/^https?:\/\//i", "");
            DeviceId = deviceId;
            ConsentToken = consentToken;
        }

        public async Task<string> GetFullAddressAsync(ILambdaLogger logger)
        {
            logger.LogLine("Starting GetFullAddress...");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConsentToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            //var requestOptions = $"/v1/devices/{DeviceId}/settings/address/";
            //var uri = new Uri($"{ApiEndpoint}{requestOptions}");
            var uri = $"https://api.amazonalexa.com/v1/devices/{DeviceId}/settings/address";

            logger.LogLine($"\n\nURI: {uri}");

            var response = await client.GetAsync(uri);

            var code = response.StatusCode;
            logger.LogLine($"\nHttpCode for AlexaDeviceAddressClient: {code}");

            //DEMO: convert if to switch
            if (code == HttpStatusCode.OK)
            {
                logger.LogLine("\nAddress successfully retrieved, now responding to user.");
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<Address>(content);
                logger.LogLine("\nAddress is: " + json.ToString());
                return json.ToString();
            }
            else if (code == HttpStatusCode.NoContent)
            {
                logger.LogLine("\nSuccessfully requested from the device address API, but no address was returned");
                return "";
            }
            else if (code == HttpStatusCode.Forbidden)
            {
                logger.LogLine("\nThe consent token we had wasn't authorized to access the user's address. ");
                return "";
            }
            else
            {
                logger.LogLine("\nUnknown location failure.");
                return "";
            }
            // DEMO remove unreachable code
            logger.LogLine("Ending GetFullAddress...");
        }


    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }

    public class Geocode
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
    public class BusInfoResource
    {
        public string Language { get; set; }
        public string SkillName { get; set; }
        public string HelpMessage { get; set; }
        public string HelpReprompt { get; set; }
        public string StopMessage { get; set; }

        public BusInfoResource(string language)
        {
            Language = language ?? throw new ArgumentNullException(nameof(language));
        }
    }
    public class Address
    {
        public string stateOrRegion { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string postalCode { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string districtOrCounty { get; set; }

        public override string ToString()
        {
            return $"{addressLine1},{city},{stateOrRegion},{postalCode}";
        }
    }


}
