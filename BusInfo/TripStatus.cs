using System.Collections.Generic;

namespace BusInfo
{
    public class TripStatus
    {
        public string ActiveTripId { get; set; }
        public int BlockTripSequence { get; set; }
        public string ClosestStop { get; set; }
        public int ClosestStopTimeOffset { get; set; }
        public double DistanceAlongTrip { get; set; }
        public object Frequency { get; set; }
        public int LastKnownDistanceAlongTrip { get; set; }
        public LastKnownLocation LastKnownLocation { get; set; }
        public int LastKnownOrientation { get; set; }
        public object LastLocationUpdateTime { get; set; }
        public object LastUpdateTime { get; set; }
        public string NextStop { get; set; }
        public int NextStopTimeOffset { get; set; }
        public double Orientation { get; set; }
        public string Phase { get; set; }
        public Position Position { get; set; }
        public bool Predicted { get; set; }
        public int ScheduleDeviation { get; set; }
        public double ScheduledDistanceAlongTrip { get; set; }
        public object ServiceDate { get; set; }
        public List<object> SituationIds { get; set; }
        public string Status { get; set; }
        public double TotalDistanceAlongTrip { get; set; }
        public string VehicleId { get; set; }
    }
}
