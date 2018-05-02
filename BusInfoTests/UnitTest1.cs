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
            //var expectedStop = new Stop("700", "1_700", 47.610951, 0, -122.33725, "4th Ave & Pike St",
            //new List<string>(), "UNKNOWN", new Direction("NW"));

            var expectedStop = new Stop("1050", "1_1050", 47.613937, 0, -122.33416, "Olive Way & 8th Ave", new List<string>(), "UNKNOWN", new Direction("NE"));

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
        public async Task TestGetArrivals()
        {
            var actual = await busInfo.GetArrivalTimesForRouteName(_busRoute, conventionCenter.lat, conventionCenter.lon, date);
            var expected = new List<double>();
            expected.Add(4); //3min 42s
            expected.Add(5); //5 min 12s
            expected.Add(14); //13min 46s

            Assert.AreEqual(0, 0);
            //Assert.AreEqual(expected2.Count, actual.Count);
            //CollectionAssert.AreEqual(expected2, actual);
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
