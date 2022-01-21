using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using Timeentry;
using Timeentry.Models;
using Xunit;

namespace TestProject1
{
    public  class FuncTest
    {

        [Fact]
        public async void TestTriger()
        {
            SetVariables();

            Msdyn_timeentry timeEntry = new Msdyn_timeentry { StartOn = "2022.01.01", EndOn = "2022.01.10" };
            Mock<HttpRequest> mockRequest = CreateMockRequest(timeEntry);
            MyLog myLog = new MyLog();
 
            IActionResult result = await TimeEntryFunction.Run(mockRequest.Object, myLog);
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResponse.StatusCode);
        }


        [Fact]
        public async void TestToken()
        {
            SetVariables();

            String token = await StaticFunctions.GetToken();
            Assert.True(token != ""); 
        }

        private static void SetVariables()
        {
            Environment.SetEnvironmentVariable("DVclientId", "c316ac1a-5759-41f5-9906-715a86552865");
            Environment.SetEnvironmentVariable("DVclientSecret", ""); //** Deleted for Public repository
            Environment.SetEnvironmentVariable("DVtenantId", ""); //** Deleted for Public repository
            Environment.SetEnvironmentVariable("DVscope", "https://org2c9fce96.api.crm4.dynamics.com/.default");
            Environment.SetEnvironmentVariable("DVLoginUrl", "https://login.microsoftonline.com");
            Environment.SetEnvironmentVariable("DVUrl", "https://org2c9fce96.api.crm4.dynamics.com/api/data/v9.2");
        }

        private static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }


    }
}
