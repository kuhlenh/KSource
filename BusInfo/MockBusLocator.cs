using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BusInfo
{
    public class MockBusLocator : IBusLocator
    {
        private static string LoadJson(string file)
        {
            return File.ReadAllText(file);
        }

        private static string GetTestProjectRootFolder()
        {
            const string ProjectName = "BusInfoTests";
            string rootPath;
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var dir = Path.GetDirectoryName(path);

            //running in the context of LUT, and the path needs to be adjusted
            if (dir.Contains(".vs"))
            {
                rootPath = $"{dir.Substring(0, dir.IndexOf("\\.vs\\") + 1)}{ProjectName}\\";
            }
            else
            {
                rootPath = dir.Substring(0, dir.IndexOf("\\bin\\") + 1);
            }

            return rootPath;
        }

        public Task<string> GetJsonForArrivals(string stopId)
        {
            var rootPath = MockBusLocator.GetTestProjectRootFolder();
            var json = LoadJson($"{rootPath}Arrivals.json");
            return Task.FromResult(json);
        }

        public Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon)
        {
            var rootPath = MockBusLocator.GetTestProjectRootFolder();
            var json = LoadJson($"{rootPath}StopsForLoc.json");
            return Task.FromResult(json);
        }
    }
}
