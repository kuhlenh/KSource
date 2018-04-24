using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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





            Type requestType = input.GetRequestType();
            if (requestType == typeof(LaunchRequest))
            {
                log.LogLine($"Default LaunchRequest made");
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = "Where would you like to go?";
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;
                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                        log.LogLine($"AMAZON.CancelIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Stopping!";
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.StopIntent":
                        log.LogLine($"AMAZON.StopIntent: send StopMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Stopping";
                        response.Response.ShouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        log.LogLine($"AMAZON.HelpIntent: send HelpMessage");
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Say the name of your bus route.";
                        break;
                    case "GetRouteIntent":
                        log.LogLine($"Intent Requested {intentRequest.Intent.Name}");
                        innerResponse = new PlainTextOutputSpeech();
                        string value = intentRequest.Intent.Slots["RouteName"].Value;
                        var location = await GetLatLonForUserLocation(input.Context.System, log);
                        var (lat, lon) = ((location.Item1.Length != 0) && (location.Item2.Length != 0)) ? location : ("40.611959", "-120.332893");
                        //("47.611959", "-122.332893")
                        MyStopInfo busInfo = new MyStopInfo(new BusLocator(), new TimeZoneConverter());
                        var arrivalTimes = await busInfo.GetArrivalTimesForRouteName(value, lat, lon);
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"The next {value} comes in ");
                        foreach (var arrival in arrivalTimes)
                        {
                            sb.Append(arrival + " minutes,");
                        }
                        (innerResponse as PlainTextOutputSpeech).Text = sb.ToString();
                        log.LogLine($"route:{value}, lat: {lat}, lon: {lon}");
                        break;
                    default:
                        log.LogLine($"Unknown intent: " + intentRequest.Intent.Name);
                        innerResponse = new PlainTextOutputSpeech();
                        (innerResponse as PlainTextOutputSpeech).Text = "Say the name of your bus route";
                        break;
                }
            }

            response.Response.OutputSpeech = innerResponse;
            response.Version = "1.0";
            return response;
        }

        private async Task<(string, string)> GetLatLonForUserLocation(AlexaSystem system, ILambdaLogger log)
        {
            var accessToken = system.ApiAccessToken;
            var deviceId = system.Device.DeviceID;

            HttpClient client = new HttpClient() { BaseAddress = new Uri("https://api.amazonalexa.com") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var uri = $"https://api.amazonalexa.com/v1/devices/{deviceId}/settings/address";
            
            var response = await client.GetAsync(uri);

            log.LogLine("\n request message: " + response.RequestMessage);
            log.LogLine("\n code: " + response.StatusCode);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<Address>(content);
                log.LogLine("\njson for address: " + json);
                var key = "AIzaSyAKLwQo-xS-a7HChxZDjBvxHxyo0vCj8RE";
                client.DefaultRequestHeaders.Clear();
                var uriGoogle = new Uri($"https://maps.google.com/maps/api/geocode/json?key={key}&address={json.addressLine1 + "," + json.city + "," + json.stateOrRegion}&sensor=false&region={json.countryCode}");
                var response2 = await client.GetAsync(uriGoogle);
                if (!response2.IsSuccessStatusCode)
                {
                    throw new Exception("Google API failed.");
                }
                var json2 = await response2.Content.ReadAsStringAsync();
                var json3 = JObject.Parse(json2);
                log.LogLine("\ngoogle: " + json3);
                return (json3["geometry"]["location"]["lat"].ToString(), json3["geometry"]["location"]["long"].ToString());
            }

            return ("", "");

        }

        public string GetRouteName(string route) => route;
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
    }


}
