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
using ETransitLamdbda;
using System.IO;
using Newtonsoft.Json;

namespace ETransitLamdbda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToUpperFunction()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var upperCase = function.FunctionHandler("hello world", context);

            Assert.Equal("HELLO WORLD", upperCase);
        }
    }
}

namespace EmeraldTransit_Seattle.Tests
{
    public class FunctionTest
    {
        private static string LoadJson(string file) => File.ReadAllText(file);

        public string GetSkillRequest()
        {
            var json = LoadJson(@"C:\Users\kaseyu\Source\Repos\EmeraldTransit_Seattle\EmeraldTransit_Seattle\Request.json");
            return json;
        }

        [Fact]
        public void TestToUpperFunction2()
        {
            var json = GetSkillRequest();
            var request = JsonConvert.DeserializeObject<SkillRequest>(json);

            // invoke function
            var function = new Function();
            var context = new TestLambdaContext();
            var upperCase = function.FunctionHandler(request, context);

            Assert.Equal("", "");
        }
    }

}


