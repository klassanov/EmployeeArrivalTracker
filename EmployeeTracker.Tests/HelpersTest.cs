using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Caching;
using EmployeeTracker.Domain.Model;
using EmployeeTracker.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EmployeeTracker.Tests
{
    [TestClass]
    public class HelpersTest
    {
        [TestMethod]
        public void StringHelper_Can_Generate_Valid_Url_For_Date()
        {
            //Arrange
            DateTime subscriptionDate = new DateTime(1985, 7, 23);

            //Act
            string url=StringHelper.CreateWebSericeSubscriptionRequestUrl(subscriptionDate);

            //Assert
            Assert.AreEqual(url, "http://localhost:51396/api/clients/subscribe?date=1985-07-23&callback=http://localhost:61051/employee/Post");
        }

        [TestMethod]
        public void SimpleTokenHelper_Can_Store_Token()
        {
            //Arrange
            SimpleTokenHelper tokenHelper = new SimpleTokenHelper();
            string tokenValue=Guid.NewGuid().ToString("N");
            SubscriptionToken subscriptionToken = new SubscriptionToken(){
                Expires = DateTime.Now.AddDays(1),
                Token = tokenValue
            };

            //Act
            tokenHelper.StoreToken(subscriptionToken);

            //Assert
            Assert.AreEqual(tokenHelper.GetToken(), subscriptionToken);
        }

        [TestMethod]
        public void SimpleTokenHelper_Can_Check_Token()
        {
            //Arrange
            SimpleTokenHelper tokenHelper = new SimpleTokenHelper();
            string validTokenValue = Guid.NewGuid().ToString("N");
            string invalidTokenValue = Guid.NewGuid().ToString("N");
            SubscriptionToken subscriptionToken = new SubscriptionToken()
            {
                Expires = DateTime.Now.AddDays(1),
                Token = validTokenValue
            };

            //Act
            tokenHelper.StoreToken(subscriptionToken);
            bool validTokenCheck=tokenHelper.CheckToken(validTokenValue);
            bool invalidTokenCheck = tokenHelper.CheckToken(invalidTokenValue);

            //Assert
            Assert.IsTrue(validTokenCheck);
            Assert.IsFalse(invalidTokenCheck);
        }       
    }
}
