using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TimeentryTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async void TestMethod1()
        {

        }

        [Fact]
        public async Task ReturnCorrectDepositInformation()
        {
            var deposit = new Deposit { Amount = 42 };
            var investor = new Investor { };

            Mock<HttpRequest> mockRequest = CreateMockRequest(deposit);

            DepositRequest result = await Portfolio.Run(mockRequest.Object, investor, "42", new Mock<ILogger>().Object);

            Assert.Equal(42, result.Amount);
            Assert.Same(investor, result.Investor);
        }

    }
}
