using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using EmeraldTransit_Seattle;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Request.Type;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace EmeraldTransit_Seattle.Tests
{
    public class FunctionTest
    {
        private static string LoadJson(string file) => File.ReadAllText(file);

        private static string GetTestProjectRootFolder()
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

        public string GetSkillRequest()
        {
            var rootPath = GetTestProjectRootFolder();
            var jsonPath = Path.Combine(rootPath, @"..\EmeraldTransit_Seattle\Request.json");
            var json = LoadJson(jsonPath);
            return json;
        }

        [Fact]
        public void TestSkill1()
        {
            var json = GetSkillRequest();
            var request = JsonConvert.DeserializeObject<SkillRequest>(json);

            // invoke function
            var function = new Function();
            var context = new TestLambdaContext();
            var response = function.FunctionHandler(request, context);

            var text = response.Result.ToString();

            Assert.Equal("", "");
        }
    }

}


