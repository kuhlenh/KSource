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
    
        MyStopInfo _busInfo = new MyStopInfo(new MockBusLocator(), new MockTimeZoneConverter());
        (string lat, string lon) _conventionCenter = ("47.611959", "-122.332893");
        (string lat, string lon) _microsoftCampus = ("47.639905", "-122.125485");
        private readonly static string _busRoute = "545";
        DateTime _date = DateTime.Parse("5/01/2018, 2:50:00 PM");

        [TestMethod]
        public async Task TestGetRouteAndStopForLocation()
        {
            var actual = await _busInfo.GetRouteAndStopForLocation(_busRoute, _conventionCenter.lat, _conventionCenter.lon);
            var expectedRoute = new Route("40", "", "Redmond Seattle", "40_100236", "Redmond - Seattle", "545",
                                          "", 3, "http://www.soundtransit.org/Schedules/ST-Express-Bus/545");

            var expectedStop = new Stop("1050", "1_1050", 47.613937, 0, -122.33416, "Olive Way & 8th Ave", new List<string>(), "UNKNOWN", new Direction("NE"));

            Assert.AreEqual(expectedStop.Id, actual.Item2.Id);
            Assert.AreEqual(expectedRoute.Id, actual.Item1.Id);
        }

        [TestMethod]
        public async Task TestGetRouteAndStopMissing()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                        await _busInfo.GetRouteAndStopForLocation("234", _conventionCenter.lat, _conventionCenter.lon));
        }

        [TestMethod]
        public async Task TestGetArrivals()
        {
            var actual = await _busInfo.GetArrivalTimesForRouteName(_busRoute, _conventionCenter.lat, _conventionCenter.lon, _date);
            var expected = new List<double>();
            expected.Add(9);
            expected.Add(12);
            expected.Add(36);

            Assert.AreEqual(expected.Count, actual.Count);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestGetArrivalsInvalidLatLon()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                                    await _busInfo.GetArrivalTimesForRouteName(_busRoute, "-100.0000", "200.0000", _date),
                                    "Not a valid latitude or longitude.");
        }
    }
}
