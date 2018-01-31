using System.Collections.Generic;

namespace BusInfo
{
    public class ArrivalsAndDeparture
    {
        public bool ArrivalEnabled { get; set; }
        public int BlockTripSequence { get; set; }
        public bool DepartureEnabled { get; set; }
        public double DistanceFromStop { get; set; }
        public object Frequency { get; set; }
        public object LastUpdateTime { get; set; }
        public int NumberOfStopsAway { get; set; }
        public bool Predicted { get; set; }
        public object PredictedArrivalInterval { get; set; }
        public object PredictedArrivalTime { get; set; }
        public object PredictedDepartureInterval { get; set; }
        public object PredictedDepartureTime { get; set; }
        public string RouteId { get; set; }
        public string RouteLongName { get; set; }
        public string RouteShortName { get; set; }
        public object ScheduledArrivalInterval { get; set; }
        public object ScheduledArrivalTime { get; set; }
        public object ScheduledDepartureInterval { get; set; }
        public object ScheduledDepartureTime { get; set; }
        public object ServiceDate { get; set; }
        public List<object> SituationIds { get; set; }
        public string Status { get; set; }
        public string StopId { get; set; }
        public int StopSequence { get; set; }
        public int TotalStopsInTrip { get; set; }
        public string TripHeadsign { get; set; }
        public string TripId { get; set; }
        public TripStatus TripStatus { get; set; }
        public string VehicleId { get; set; }
    }
}
