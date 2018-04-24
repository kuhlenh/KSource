using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusInfo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
   
    [TestClass]
    public class UnitTest1
    {
    
        MyStopInfo busInfo = new MyStopInfo(new MockBusLocator(), new MockTimeZoneConverter());
        (string lat, string lon) conventionCenter = ("47.611959", "-122.332893");
        (string lat, string lon) microsoftCampus = ("47.639905", "-122.125485");
        string _busRoute = "545";
        DateTime date = DateTime.Parse("5/1/2017, 4:30:00 PM");

        [TestMethod]
        public async Task TestGetRouteAndStopForLocation()
        {
            var actual = await busInfo.GetRouteAndStopForLocation(_busRoute, conventionCenter.lat, conventionCenter.lon);
            var expectedRoute = new Route("40", "", "Redmond Seattle", "40_100236", "Redmond - Seattle", "545",
                                          "", 3, "http://www.soundtransit.org/Schedules/ST-Express-Bus/545");
            var expectedStop = new Stop("700", "1_700", 47.610951, 0, -122.33725, "4th Ave & Pike St",
                                        new List<string>(), "UNKNOWN", new Direction("NW"));

            Assert.AreEqual(expectedStop.Id, actual.Item2.Id);
            Assert.AreEqual(expectedRoute.Id, actual.Item1.Id);
        }

        [TestMethod]
        public async Task TestGetRouteAndStopMissing()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                        await busInfo.GetRouteAndStopForLocation("234", conventionCenter.lat, conventionCenter.lon));
        }

        [TestMethod]
        public async Task TestGetTimeZoneInfoAsync()
        {
            var actual = await busInfo.GetTimeZoneInfoAsync(conventionCenter.lat, conventionCenter.lon);

            //await Assert.AreEqual(3, 3);
        }

        [TestMethod]
        public async Task TestGetArrivals()
        {
            var actual = await busInfo.GetArrivalTimesForRouteName(_busRoute, conventionCenter.lat, conventionCenter.lon, date);
            var expected = new List<DateTime>();
            expected.Add(DateTime.Parse("5/1/2017, 4:33:42 PM"));
            expected.Add(DateTime.Parse("5/1/2017, 4:35:12 PM"));
            expected.Add(DateTime.Parse("5/1/2017, 4:43:46 PM"));

            var expected2 = new List<double>(); // DateTime.Parse("5/1/2017, 4:30:00 PM");
            expected2.Add(3.7); //3min 42s
            expected2.Add(5.2); //5 min 12s
            expected2.Add(13.8); //13min 46s

            Assert.AreEqual(3, actual.Count);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestGetArrivalsInvalidLatLon()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                                    await busInfo.GetArrivalTimesForRouteName(_busRoute, "-100.0000", "200.0000", date),
                                    "Not a valid latitude or longitude.");
        }

        [TestMethod]
        public async Task TestGetArrivalsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                                    await busInfo.GetArrivalTimesForRouteName(_busRoute, 
                                    null, null, date));
        }
    }
}
