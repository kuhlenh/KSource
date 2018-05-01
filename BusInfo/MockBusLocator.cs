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

        internal static string GetTestProjectRootFolder()
        {
            string rootPath;
            Assembly asm = Assembly.GetExecutingAssembly();
            string codeBase = asm.CodeBase;
            string projectName = asm.GetName().Name;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var dir = Path.GetDirectoryName(path);

            //running in the context of LUT, and the path needs to be adjusted
            if (dir.Contains(".vs"))
            {
                rootPath = $"{dir.Substring(0, dir.IndexOf("\\.vs\\") + 1)}{projectName}\\";
            }
            else
            { 
                var projPath = dir.Substring(0, dir.IndexOf("\\bin\\") + 1);
                rootPath = Path.Combine(projPath, @"..", projectName);
            }

            return rootPath;
        }

        public Task<string> GetJsonForArrivals(string stopId)
        {
            var rootPath = MockBusLocator.GetTestProjectRootFolder();
            var json = LoadJson(Path.Combine(rootPath, "Arrivals.json"));
            return Task.FromResult(json);
        }

        public Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon)
        {
            var rootPath = MockBusLocator.GetTestProjectRootFolder();
            var json = LoadJson(Path.Combine(rootPath, "StopsForLoc.json"));
            return Task.FromResult(json);
        }
    }
}
