﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EmeraldTransit_Seattle
{
    public interface IAlexaDeviceAddressClient
    {
        string ApiEndpoint { get; set; }
        string DeviceId { get; set; }
        string ConsentToken { get; set; }
        Task<string> GetFullAddressAsync(ILogger logger);
    }

    class AlexaDeviceAddressClient : IAlexaDeviceAddressClient
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

        public async Task<string> GetFullAddressAsync(ILogger logger)
        {
            logger.LogLine("Starting GetFullAddress...");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConsentToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var uri = $"https://api.amazonalexa.com/v1/devices/{DeviceId}/settings/address";

            logger.LogLine(String.Format("\n\nURI: {0}", uri));

            var response = await client.GetAsync(uri);

            var code = response.StatusCode;
            logger.LogLine(String.Format("\nHttpCode for AlexaDeviceAddressClient: {0}", code));

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

            logger.LogLine("Ending GetFullAddress...");
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

    public class MockAlexaDeviceAddressClient : IAlexaDeviceAddressClient
    {
        public MockAlexaDeviceAddressClient()
        {
            ApiEndpoint = "";
            DeviceId = "";
            ConsentToken = "";
        }

        public MockAlexaDeviceAddressClient(string apiEndpoint, string deviceId, string consentToken)
        {
            ApiEndpoint = apiEndpoint;
            DeviceId = deviceId;
            ConsentToken = consentToken;
        }

        public string ApiEndpoint { get; set; }
        public string DeviceId { get; set; }
        public string ConsentToken { get; set; }

        public Task<string> GetFullAddressAsync(ILogger logger)
        {
            return Task.FromResult("705 Pike St,Seattle,WA,98101");
        }
    }
}