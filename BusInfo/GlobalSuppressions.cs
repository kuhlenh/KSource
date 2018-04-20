
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>", Scope = "member", Target = "~M:EmeraldTransit_Seattle.A.Dispose")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "<Pending>", Scope = "member", Target = "~M:BusInfo.Route.#ctor(System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Int32,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>", Scope = "member", Target = "~P:BusInfo.ArrivalsAndDeparture.SituationIds")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>", Scope = "member", Target = "~P:BusInfo.Route.Url")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>", Scope = "member", Target = "~P:BusInfo.Stop.RouteIds")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>", Scope = "member", Target = "~P:BusInfo.TripStatus.SituationIds")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "<Pending>", Scope = "member", Target = "~M:BusInfo.BusLocator.GetJsonForArrivals(System.String)~System.Threading.Tasks.Task{System.String}")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "<Pending>", Scope = "member", Target = "~M:BusInfo.BusLocator.GetJsonForStopsFromLatLongAsync(System.String,System.String)~System.Threading.Tasks.Task{System.String}")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2200:Rethrow to preserve stack details.", Justification = "<Pending>", Scope = "member", Target = "~M:BusInfo.MyStopInfo.GetTimeZoneInfoAsync(System.String,System.String)~System.Threading.Tasks.Task{System.TimeZoneInfo}")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings", Justification = "<Pending>", Scope = "member", Target = "~M:BusInfo.TimeZoneConverter.GetTimeZoneJsonFromLatLonAsync(System.String,System.String)~System.Threading.Tasks.Task{System.String}")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:EmeraldTransit_Seattle.Test.M1(EmeraldTransit_Seattle.A,System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>", Scope = "namespace", Target = "~N:EmeraldTransit_Seattle")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>", Scope = "type", Target = "~T:BusInfo.Stop")]