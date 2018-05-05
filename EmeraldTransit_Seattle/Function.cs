using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using BusInfo;
using Newtonsoft.Json.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace EmeraldTransit_Seattle
{

    public class Function
    {
        HttpClient client = new HttpClient();
        private IMapLocator _mapLocator;
        private IAlexaDeviceAddressClient _deviceAddressClient;
        private MyStopInfo _busInfo;

        public Function(IMapLocator mapLocator, IAlexaDeviceAddressClient deviceAddressClient)
        {
            _mapLocator = mapLocator;
            _deviceAddressClient = deviceAddressClient;
        }

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
            var log = (ILogger)context.Logger;

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
                        if (_deviceAddressClient is MockAlexaDeviceAddressClient)
                        {
                            _deviceAddressClient = new MockAlexaDeviceAddressClient(apiEndpoint, deviceId, consentToken);
                            _busInfo = new MyStopInfo(new MockBusLocator(), new MockTimeZoneConverter());
                        }
                        else
                        {
                            _deviceAddressClient = new AlexaDeviceAddressClient(apiEndpoint, deviceId, consentToken);
                            _busInfo = new MyStopInfo(new BusLocator(), new TimeZoneConverter());
                        }
                        var address = await _deviceAddressClient.GetFullAddressAsync(log);
                        log.LogLine("==================================");
                        log.LogLine("\nAddress: " + address);
                        log.LogLine("==================================");
                        
                        // call into the Google Maps api to get geocode location
                        var location = await _mapLocator.GoogleMapGeocodeLocation(log, address);

                        // call into the OBA api to get bus times
                        var sb = await GetBusResponse(route, location, time, _busInfo);
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
            var plainTextOutput = (innerResponse as PlainTextOutputSpeech);
            plainTextOutput.Text = plainTextOutput.Text != "testing" ? plainTextOutput.Text : "No stops could be found within a mile of your location.";
            response.Response.OutputSpeech = innerResponse;
                response.Version = "1.0";
            return response;
        }

        private static async Task<string> GetBusResponse(string route, (string, string) location, DateTime time, MyStopInfo busInfo)
        {
            var arrivalTimes = await busInfo.GetArrivalTimesForRouteName(route, location.Item1, location.Item2, time);
            StringBuilder sb = new StringBuilder();
            sb.Append($"The next {route} comes in ");

            foreach (var arrival in arrivalTimes)
            {
                sb.Append(arrival + " minutes,");
            }

            return sb.ToString();
        }
    } 
}
