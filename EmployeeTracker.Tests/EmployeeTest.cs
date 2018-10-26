using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EmployeeTracker.DataAccess.Interfaces;
using EmployeeTracker.Domain.Model;
using EmployeeTracker.Web.Controllers;
using EmployeeTracker.Web.Helpers;
using Flurl.Http.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace EmployeeTracker.Tests
{
    [TestClass]
    public class EmployeeTest
    {
        private Mock<IEmployeeTrackerRepository> repository;
        private Mock<ITokenHelper> tokenHelper;

        private int employeesArrivalsNumber;
        private int postRequestsNumber;
        private int subscriptionRequestsNumber;

        private DateTime startDateTime;
        private List<EmployeeArrival> employeeArrivals;
        private List<EmployeeArrivalPostRequest> postRequests;
        private List<EmployeeArrivalSubscriptionGetRequest> subscriptionRequests;

        public EmployeeTest()
        {
            Initialize();
        }

        private void Initialize()
        {
            startDateTime = DateTime.Now;
            employeesArrivalsNumber = 10;
            postRequestsNumber = 4;
            subscriptionRequestsNumber = 1;

            employeeArrivals = new List<EmployeeArrival>();
            postRequests = new List<EmployeeArrivalPostRequest>();
            subscriptionRequests = new List<EmployeeArrivalSubscriptionGetRequest>();

            for (int i = 0; i < employeesArrivalsNumber; i++)
            {
                employeeArrivals.Add(new EmployeeArrival { Id = i, EmployeeId = i, When = startDateTime.AddMinutes(1) });
            }

            for (int i = 0; i < postRequestsNumber; i++)
            {
                postRequests.Add(new EmployeeArrivalPostRequest { Id = i, TokenValue = Guid.NewGuid().ToString("N"), IsValid = i % 2 == 0 });
            }

            for (int i = 0; i < subscriptionRequestsNumber; i++)
            {
                subscriptionRequests.Add(new EmployeeArrivalSubscriptionGetRequest { Id = i, ResponseStatusCode = 200, DateParameter = startDateTime });
            }

            //Mock the repository
            repository = new Mock<IEmployeeTrackerRepository>();
            repository.Setup(m => m.GetAll<EmployeeArrival>(null)).Returns(employeeArrivals);
            
            repository.Setup(m => m.GetAll<EmployeeArrivalPostRequest>(null)).Returns(postRequests);
            repository.Setup(m => m.GetAll<EmployeeArrivalSubscriptionGetRequest>(null)).Returns(subscriptionRequests);

            //Mock the token helper
            tokenHelper = new Mock<ITokenHelper>();
            tokenHelper.Setup(m => m.CheckToken(It.IsAny<string>())).Returns(true);
        }

        [TestMethod]
        public void Arrivals_Contains_All_EmployeeArrivals()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

            //Act
            ViewResult viewResult = controller.Arrivals();
            IEnumerable<EmployeeArrival> viewModel = viewResult.Model as IEnumerable<EmployeeArrival>;

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Count(), employeesArrivalsNumber);
            Assert.AreEqual(viewModel.ElementAt(0).EmployeeId, 0);
            Assert.AreEqual(viewModel.Last().EmployeeId, employeesArrivalsNumber - 1);
        }
                      
        [TestMethod]
        public void ArrivalsById_Contains_Only_Filtered_Entities()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

            repository.Setup(m => m.GetAll<EmployeeArrival>(It.IsAny<Func<EmployeeArrival, bool>>()))
                .Returns<Func<EmployeeArrival, bool>>(whereCondition => employeeArrivals.Where(whereCondition));

            //Act
            int employeeId = 3;
            ViewResult viewResult = controller.ArrivalsById(employeeId);
            IEnumerable<EmployeeArrival> viewModel = viewResult.Model as IEnumerable<EmployeeArrival>;
        }
       
        [TestMethod]
        public void PostRequests_Contains_All_Entities()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

            //Act
            ViewResult viewResult = controller.PostRequests();
            IEnumerable<EmployeeArrivalPostRequest> viewModel = viewResult.Model as IEnumerable<EmployeeArrivalPostRequest>;

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Count(), postRequestsNumber);
            Assert.AreEqual(viewModel.ElementAt(0).Id, 0);
            Assert.AreEqual(viewModel.Last().Id, postRequestsNumber - 1);
        }

        [TestMethod]
        public void SubscriptionsHistory_Contains_All_Entities()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

            //Act
            ViewResult viewResult = controller.SubscriptionsHistory();
            IEnumerable<EmployeeArrivalSubscriptionGetRequest> viewModel = viewResult.Model as IEnumerable<EmployeeArrivalSubscriptionGetRequest>;

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Count(), subscriptionRequestsNumber);
            Assert.AreEqual(viewModel.ElementAt(0).Id, 0);
            Assert.AreEqual(viewModel.Last().Id, subscriptionRequestsNumber - 1);
        }

        [TestMethod]
        public void Subscribe_Can_Render_View()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

            //Act
            ViewResult viewResult = controller.Subscribe();

            //Assert
            Assert.IsNotNull(viewResult);
        }
        
        
        [TestMethod]
        public async Task Subscribe_Can_Send_Subscription_Request_With_Valid_Input_Data()
        {
            //Arrange
            DateTime dateParameter = new DateTime(1985, 7, 23);
            string tokenValue = Guid.NewGuid().ToString("N");
            EmployeeArrivalSubscriptionGetRequest subscriptionRequest = new EmployeeArrivalSubscriptionGetRequest(){
                DateParameter=dateParameter
            };       
            SubscriptionToken subscriptionToken = new SubscriptionToken{
                Expires = DateTime.Now.AddDays(1),
                Token = tokenValue
            };

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(JsonConvert.SerializeObject(subscriptionToken), 200);
                EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

                //Act
                ViewResult viewResult=await controller.Subscribe(subscriptionRequest);

                //Assert
                httpTest.ShouldHaveCalled(StringHelper.CreateWebSericeSubscriptionRequestUrl(subscriptionRequest.DateParameter))
                    .WithHeader(ConfigurationManager.AppSettings["ServiceRequestHeader"], ConfigurationManager.AppSettings["ServiceRequestHeaderValue"])
                    .WithVerb(HttpMethod.Get)
                    .Times(1);
            }           
        }

        [TestMethod]
        public async Task Subscribe_Cannot_Send_SubscriptionRequest_With_Invalid_Input_Data()
        {
            //Arrange
            DateTime dateParameter = new DateTime(1985, 7, 23);
            string tokenValue = Guid.NewGuid().ToString("N");
            EmployeeArrivalSubscriptionGetRequest subscriptionRequest = new EmployeeArrivalSubscriptionGetRequest();
            SubscriptionToken subscriptionToken = new SubscriptionToken
            {
                Expires = DateTime.Now.AddDays(1),
                Token = tokenValue
            };

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(JsonConvert.SerializeObject(subscriptionToken), 200);
                EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);
                controller.ModelState.AddModelError("Data", "Data is required field");

                //Act
                ViewResult viewResult = await controller.Subscribe(subscriptionRequest);

                //Assert
                httpTest.ShouldNotHaveCalled(StringHelper.CreateWebSericeSubscriptionRequestUrl(subscriptionRequest.DateParameter));
            }
        }


        [TestMethod]
        public void Post_Can_Save_Valid_Requests()
        {
            //Arrange
            string token = Guid.NewGuid().ToString("N");
            Mock<HttpRequestBase> webServicePostRequest = new Mock<HttpRequestBase>();
            webServicePostRequest.SetupGet(x => x.Headers).Returns(
                    new System.Net.WebHeaderCollection {
                        { ConfigurationManager.AppSettings["xFourthTokenHeader"], token}
                    });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(webServicePostRequest.Object);
            EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

            //Act
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.Post(employeeArrivals);

            //Assert
            repository.Verify(m => m.WriteArrivalPostRequest(It.Is<EmployeeArrivalPostRequest>(x => x.TokenValue == token), employeeArrivals));
        }

        [TestMethod]
        public void Post_Cant_Save_Requests_Without_Header()
        {
            //Arrange
            string token = Guid.NewGuid().ToString("N");
            Mock<HttpRequestBase> webServicePostRequest = new Mock<HttpRequestBase>();
            webServicePostRequest.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection());
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(webServicePostRequest.Object);
            EmployeeController controller = new EmployeeController(repository.Object, tokenHelper.Object);

            //Act
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.Post(employeeArrivals);

            //Assert           
            repository.Verify(m => m.WriteArrivalPostRequest(It.IsAny<EmployeeArrivalPostRequest>(), null));
        }

        [TestMethod]
        public void Post_Cant_Save_Employees_Arrivals_With_Invalid_Token()
        {
            //Arrange
            string token = Guid.NewGuid().ToString("N");
            Mock<HttpRequestBase> webServicePostRequest = new Mock<HttpRequestBase>();
            webServicePostRequest.SetupGet(x => x.Headers).Returns(
                    new System.Net.WebHeaderCollection {
                        { ConfigurationManager.AppSettings["xFourthTokenHeader"], token}
                    });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(webServicePostRequest.Object);

            Mock<ITokenHelper> tokenHelperFail = new Mock<ITokenHelper>();
            tokenHelperFail.Setup(m => m.CheckToken(It.IsAny<string>())).Returns(false);

            EmployeeController controller = new EmployeeController(repository.Object, tokenHelperFail.Object);

            //Act
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.Post(employeeArrivals);

            //Assert           
            repository.Verify(m => m.WriteArrivalPostRequest(It.IsAny<EmployeeArrivalPostRequest>(), null));
        }
    }
}
