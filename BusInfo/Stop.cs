using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BusInfo
{
    public class Stop
    {
        public Stop(
            string code, string id, double lat, int locationType, double lon,
            string name, List<string> routeIds, string wheelchairBoarding, Direction direction)
        {
            Code = code;
            Id = id;
            Lat = lat;
            LocationType = locationType;
            Lon = lon;
            Name = name;
            RouteIds = routeIds;
            WheelchairBoarding = wheelchairBoarding;
            Direction = direction ?? throw new ArgumentNullException(nameof(direction));
        }

        public string Code { get; set; }
        public string Id { get; set; }
        public double Lat { get; set; }
        public int LocationType { get; set; }
        public double Lon { get; set; }
        public string Name { get; set; }
        public List<string> RouteIds { get; set; }
        public string WheelchairBoarding { get; set; }

        [JsonConverter(typeof(DirectionConverter))]
        public Direction Direction { get; }
    }
}
