using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
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
            //var r = GetResult(new { X = 19, Y = new { input } });

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
                        var (lat, lon) = await GetLatLonForUserLocation(input.Context.System, log);
                        //MyStopInfo busInfo = new MyStopInfo(new BusLocator(), new TimeZoneConverter());
                        int MyStopZebra = 7;
                        
                        var arrivalTimes = await busInfo.GetArrivalTimesForRouteName(value, lat, lon);
                        (innerResponse as PlainTextOutputSpeech).Text = GetRouteName($"v:{value}");
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
            HttpClient http = new HttpClient();
            var accessToken = system.ApiAccessToken;
            var deviceId = system.Device.DeviceID;
            log.LogLine($"accessToken: {accessToken}, id:{deviceId}");
            log.LogLine($"endpoint:{system.ApiEndpoint}");

            var uri = $"https://api.amazonalexa.com/v1/devices/{deviceId}/settings/address";
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await http.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                log.LogLine($"content:{response.Content}");
                var resp = await response.Content.ReadAsStringAsync();
                log.LogLine($"Deserialization: {resp}");
                var jsonAddress = JObject.Parse(resp);
                var street = jsonAddress["addressLine1"];
                var city = jsonAddress["city"];
                var state = jsonAddress["state"];
                var googleKey = "";
                var googleUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={street},{city},+{state}&key={googleKey}";
                var googleResponse = await http.GetAsync(googleUrl);
                if (!googleResponse.IsSuccessStatusCode)
                {
                    return ("47.611959", "-122.332893");
                }
                var json = await response.Content.ReadAsStringAsync();
                var results = JObject.Parse(json);
                var lat = results["results"]["geometry"]["location"]["latitude"].ToString();
                var lon = results["results"]["geometry"]["location"]["longitude"].ToString();
                return (lat, lon);

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
}
