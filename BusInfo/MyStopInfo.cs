using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusInfo
{
    public class MyStopInfo
    {
        private readonly IBusLocator _busLocator;
        private readonly ITimeZoneConverter _timezoneConverter;
        // src: http://stackoverflow.com/questions/5996320/net-timezoneinfo-from-olson-time-zone
        readonly Dictionary<string, string> _olsonWindowsTimes = new Dictionary<string, string>()
        {
            { "Africa/Bangui", "W. Central Africa Standard Time" },
            { "Africa/Cairo", "Egypt Standard Time" },
            { "Africa/Casablanca", "Morocco Standard Time" },
            { "Africa/Harare", "South Africa Standard Time" },
            { "Africa/Johannesburg", "South Africa Standard Time" },
            { "Africa/Lagos", "W. Central Africa Standard Time" },
            { "Africa/Monrovia", "Greenwich Standard Time" },
            { "Africa/Nairobi", "E. Africa Standard Time" },
            { "Africa/Windhoek", "Namibia Standard Time" },
            { "America/Anchorage", "Alaskan Standard Time" },
            { "America/Argentina/San_Juan", "Argentina Standard Time" },
            { "America/Asuncion", "Paraguay Standard Time" },
            { "America/Bahia", "Bahia Standard Time" },
            { "America/Bogota", "SA Pacific Standard Time" },
            { "America/Buenos_Aires", "Argentina Standard Time" },
            { "America/Caracas", "Venezuela Standard Time" },
            { "America/Cayenne", "SA Eastern Standard Time" },
            { "America/Chicago", "Central Standard Time" },
            { "America/Chihuahua", "Mountain Standard Time (Mexico)" },
            { "America/Cuiaba", "Central Brazilian Standard Time" },
            { "America/Denver", "Mountain Standard Time" },
            { "America/Fortaleza", "SA Eastern Standard Time" },
            { "America/Godthab", "Greenland Standard Time" },
            { "America/Guatemala", "Central America Standard Time" },
            { "America/Halifax", "Atlantic Standard Time" },
            { "America/Indianapolis", "US Eastern Standard Time" },
            { "America/Indiana/Indianapolis", "US Eastern Standard Time" },
            { "America/La_Paz", "SA Western Standard Time" },
            { "America/Los_Angeles", "Pacific Standard Time" },
            { "America/Mexico_City", "Mexico Standard Time" },
            { "America/Montevideo", "Montevideo Standard Time" },
            { "America/New_York", "Eastern Standard Time" },
            { "America/Noronha", "UTC-02" },
            { "America/Phoenix", "US Mountain Standard Time" },
            { "America/Regina", "Canada Central Standard Time" },
            { "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
            { "America/Santiago", "Pacific SA Standard Time" },
            { "America/Sao_Paulo", "E. South America Standard Time" },
            { "America/St_Johns", "Newfoundland Standard Time" },
            { "America/Tijuana", "Pacific Standard Time" },
            { "Antarctica/McMurdo", "New Zealand Standard Time" },
            { "Atlantic/South_Georgia", "UTC-02" },
            { "Asia/Almaty", "Central Asia Standard Time" },
            { "Asia/Amman", "Jordan Standard Time" },
            { "Asia/Baghdad", "Arabic Standard Time" },
            { "Asia/Baku", "Azerbaijan Standard Time" },
            { "Asia/Bangkok", "SE Asia Standard Time" },
            { "Asia/Beirut", "Middle East Standard Time" },
            { "Asia/Calcutta", "India Standard Time" },
            { "Asia/Colombo", "Sri Lanka Standard Time" },
            { "Asia/Damascus", "Syria Standard Time" },
            { "Asia/Dhaka", "Bangladesh Standard Time" },
            { "Asia/Dubai", "Arabian Standard Time" },
            { "Asia/Irkutsk", "North Asia East Standard Time" },
            { "Asia/Jerusalem", "Israel Standard Time" },
            { "Asia/Kabul", "Afghanistan Standard Time" },
            { "Asia/Kamchatka", "Kamchatka Standard Time" },
            { "Asia/Karachi", "Pakistan Standard Time" },
            { "Asia/Katmandu", "Nepal Standard Time" },
            { "Asia/Kolkata", "India Standard Time" },
            { "Asia/Krasnoyarsk", "North Asia Standard Time" },
            { "Asia/Kuala_Lumpur", "Singapore Standard Time" },
            { "Asia/Kuwait", "Arab Standard Time" },
            { "Asia/Magadan", "Magadan Standard Time" },
            { "Asia/Muscat", "Arabian Standard Time" },
            { "Asia/Novosibirsk", "N. Central Asia Standard Time" },
            { "Asia/Oral", "West Asia Standard Time" },
            { "Asia/Rangoon", "Myanmar Standard Time" },
            { "Asia/Riyadh", "Arab Standard Time" },
            { "Asia/Seoul", "Korea Standard Time" },
            { "Asia/Shanghai", "China Standard Time" },
            { "Asia/Singapore", "Singapore Standard Time" },
            { "Asia/Taipei", "Taipei Standard Time" },
            { "Asia/Tashkent", "West Asia Standard Time" },
            { "Asia/Tbilisi", "Georgian Standard Time" },
            { "Asia/Tehran", "Iran Standard Time" },
            { "Asia/Tokyo", "Tokyo Standard Time" },
            { "Asia/Ulaanbaatar", "Ulaanbaatar Standard Time" },
            { "Asia/Vladivostok", "Vladivostok Standard Time" },
            { "Asia/Yakutsk", "Yakutsk Standard Time" },
            { "Asia/Yekaterinburg", "Ekaterinburg Standard Time" },
            { "Asia/Yerevan", "Armenian Standard Time" },
            { "Atlantic/Azores", "Azores Standard Time" },
            { "Atlantic/Cape_Verde", "Cape Verde Standard Time" },
            { "Atlantic/Reykjavik", "Greenwich Standard Time" },
            { "Australia/Adelaide", "Cen. Australia Standard Time" },
            { "Australia/Brisbane", "E. Australia Standard Time" },
            { "Australia/Darwin", "AUS Central Standard Time" },
            { "Australia/Hobart", "Tasmania Standard Time" },
            { "Australia/Perth", "W. Australia Standard Time" },
            { "Australia/Sydney", "AUS Eastern Standard Time" },
            { "Etc/GMT", "UTC" },
            { "Etc/GMT+11", "UTC-11" },
            { "Etc/GMT+12", "Dateline Standard Time" },
            { "Etc/GMT+2", "UTC-02" },
            { "Etc/GMT-12", "UTC+12" },
            { "Europe/Amsterdam", "W. Europe Standard Time" },
            { "Europe/Athens", "GTB Standard Time" },
            { "Europe/Belgrade", "Central Europe Standard Time" },
            { "Europe/Berlin", "W. Europe Standard Time" },
            { "Europe/Brussels", "Romance Standard Time" },
            { "Europe/Budapest", "Central Europe Standard Time" },
            { "Europe/Dublin", "GMT Standard Time" },
            { "Europe/Helsinki", "FLE Standard Time" },
            { "Europe/Istanbul", "GTB Standard Time" },
            { "Europe/Kiev", "FLE Standard Time" },
            { "Europe/London", "GMT Standard Time" },
            { "Europe/Minsk", "E. Europe Standard Time" },
            { "Europe/Moscow", "Russian Standard Time" },
            { "Europe/Paris", "Romance Standard Time" },
            { "Europe/Sarajevo", "Central European Standard Time" },
            { "Europe/Warsaw", "Central European Standard Time" },
            { "Indian/Mauritius", "Mauritius Standard Time" },
            { "Pacific/Apia", "Samoa Standard Time" },
            { "Pacific/Auckland", "New Zealand Standard Time" },
            { "Pacific/Fiji", "Fiji Standard Time" },
            { "Pacific/Guadalcanal", "Central Pacific Standard Time" },
            { "Pacific/Guam", "West Pacific Standard Time" },
            { "Pacific/Honolulu", "Hawaiian Standard Time" },
            { "Pacific/Pago_Pago", "UTC-11" },
            { "Pacific/Port_Moresby", "West Pacific Standard Time" },
            { "Pacific/Tongatapu", "Tonga Standard Time" }
        };

        public MyStopInfo(IBusLocator busLocator, ITimeZoneConverter timezoneConverter)
        {
            _busLocator = busLocator;
            _timezoneConverter = timezoneConverter;
        }

        // Finds the closest stop for the given route name and gets arrival data for that stop
        // Returns a list of DateTimes for the timezone of the given lat/lon
        public async Task<List<DateTime>> GetArrivalTimesForRouteName(string routeShortName, string lat, string lon)
        {
            BusHelpers.ValidateLatLon(lat, lon);
            // find the route object for the given name and the closest stop for that route
            (Route route, Stop stop) = await GetRouteAndStopForLocation(routeShortName, lat, lon);

            List<ArrivalsAndDeparture> arrivalData = await GetArrivalsAndDepartures(stop.Id, route.ShortName);

            IEnumerable<DateTime> UtcData = arrivalData.Select(a => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                                               .AddMilliseconds(Convert.ToDouble(a.PredictedArrivalTime))).Take(3);
            // Convert from UTC to user's timezone
            TimeZoneInfo timeZoneInfo = await GetTimeZoneInfoAsync(lat, lon);
            IEnumerable<DateTime> UserTimeData = UtcData.Select(d => TimeZoneInfo.ConvertTimeFromUtc(d, timeZoneInfo));

            return UserTimeData.ToList();
        }

        public async Task<TimeZoneInfo> GetTimeZoneInfoAsync(string lat, string lon)
        {
            string json = await _timezoneConverter.GetTimeZoneJsonFromLatLonAsync(lat, lon);
            string timeZoneId = JObject.Parse(json)["timeZoneId"].ToString();
            try
            {
                string olsonTimeZone = _olsonWindowsTimes[timeZoneId];
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(olsonTimeZone);
                return timeZoneInfo;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<ArrivalsAndDeparture>> GetArrivalsAndDepartures(string stopId, string routeShortName)
        {
            string json = await _busLocator.GetJsonForArrivals(stopId);
            return FindArrivalsForRoute(routeShortName, stopId, json);
        }

        // Returns the arrivals and departure data if it contains the route name 
        public List<ArrivalsAndDeparture> FindArrivalsForRoute(string routeShortName, string stopId, string json)
        {
            List<ArrivalsAndDeparture> arrivalsAndDeparture = new List<ArrivalsAndDeparture>();
            JObject jobject = JObject.Parse(json);
            if (jobject["code"].ToString() == "200")
            {
                List<JToken> results = jobject["data"]["entry"]["arrivalsAndDepartures"].Children().ToList();
                IEnumerable<JToken> searchResult = results.Where(x => BusHelpers.CleanRouteName(x["routeShortName"].ToString()) == routeShortName);
                if (searchResult.Count() > 0)
                {
                    foreach (JToken s in searchResult)
                    {
                        ArrivalsAndDeparture x = s.ToObject<ArrivalsAndDeparture>();
                        arrivalsAndDeparture.Add(x);
                    }
                }
            }
            return arrivalsAndDeparture;
        }

        // Finds the bus route that matches the route short name and finds the closest
        // bus stop that contains the route.
        // Returns a tuple of the user's Route and the nearest Stop in a 1800-meter radius
        public async Task<(Route route, Stop stop)> GetRouteAndStopForLocation(string routeShortName, string lat, string lon)
        {
            (Route, List<Stop>) routeAndStops = await GetStopsForRoute(routeShortName, lat, lon);
            if (routeAndStops.Item1 == null || routeAndStops.Item2 == null)
            {
                throw new ArgumentException("No stops were found within a mile of your location for your bus route.");
            }

            Stop minDistStop = routeAndStops.Item2.First();
            return (routeAndStops.Item1, minDistStop);
        }

        private async Task<(Route, List<Stop>)> GetStopsForRoute(string routeShortName, string lat, string lon)
        {
            string json = await _busLocator.GetJsonForStopsFromLatLongAsync(lat, lon);
            return FindStopsForRoute(routeShortName, json);
        }

        // Retrieves all bus stops that contain a given route with route short name
        // Returns a tuple of the route and the associated stops in a 1800-meter radius 
        public (Route, List<Stop>) FindStopsForRoute(string routeShortName, string json)
        {
            JObject jobject = JObject.Parse(json);
            if (jobject["code"].ToString() == "200")
            {
                JEnumerable<JToken> routes = jobject["data"]["references"]["routes"].Children();
                JToken targetRoute = routes.FirstOrDefault(x => x["shortName"].ToString() == routeShortName);
                if (targetRoute != null)
                {
                    Route route = targetRoute.ToObject<Route>();
                    JEnumerable<JToken> stops = jobject["data"]["list"].Children();
                    List<Stop> stopsForRoute = new List<Stop>();
                    foreach (JToken s in stops)
                    {
                        JToken routeIds = s["routeIds"];
                        foreach (JToken rId in routeIds)
                        {
                            if (rId.ToString() == route.Id)
                            {
                                stopsForRoute.Add(s.ToObject<Stop>());
                                stopsForRoute.Add(s.ToObject<Stop>());
                            }
                        }
                    }
                    return (route, stopsForRoute);
                }
            }
            return (null, null);
        }
    }
}
