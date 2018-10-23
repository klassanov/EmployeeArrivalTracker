using EmployeeTracker.DataAccess.Interfaces;
using EmployeeTracker.Domain.Model;
using EmployeeTracker.Web.Helpers;
using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeTracker.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private IArrivalsRepository repository;
        private ITokenHelper tokenHelper;

        public EmployeeController(IArrivalsRepository repository, ITokenHelper tokenHelper)
        {
            this.repository = repository;
            this.tokenHelper = tokenHelper;
        }

        public ActionResult Index()
        {
            //HttpContext.Items
            return View();
        }

        [HttpPost]
        public void Post(IEnumerable<EmployeeArrival> arrivals)
        {
            string xFourthTokenHeader = ConfigurationManager.AppSettings["xFourthTokenHeader"];
            string tokenValue = Request.Headers[xFourthTokenHeader];
          
            EmployeeArrivalPostRequest postRequest = new EmployeeArrivalPostRequest
            {
                ReceiveDateTime=DateTime.Now,
                IsValid= tokenHelper.CheckToken(tokenValue),
                TokenValue=tokenValue
            };
            repository.WriteArrivalPostRequest(postRequest, arrivals);
        }

        public async Task<ViewResult> List()
        {
            EmployeeArrivalSubscriptionGetRequest subscriptionRequest = new EmployeeArrivalSubscriptionGetRequest();
            subscriptionRequest.CallbackUrlParameter = "http://localhost:51396/api/clients/subscribe?date=2016-03-10&callback=http://localhost:61051/employee/Post";
            subscriptionRequest.DateParameter = new DateTime(1985, 7, 23);
            HttpResponseMessage response = await GetServiceResponseMessage();
            subscriptionRequest.ResponseStatusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                SubscriptionToken subscriptionToken = JsonConvert.DeserializeObject<SubscriptionToken>(content);
                tokenHelper.StoreToken(subscriptionToken);
                subscriptionRequest.SubscriptionTokenValue = subscriptionToken.Token;
                subscriptionRequest.SubscriptionTokenExpiryDate = subscriptionToken.Expires;
            }
            else
            {

            }

            repository.WriteSubscriptionRequest(subscriptionRequest);

            return View();
        }

        private async Task<HttpResponseMessage> GetServiceResponseMessage()
        {
            //string url ="http://localhost:51396/api/clients/subscribe?date=2016-03-10&callback=http://localhost:61051/api/arrivals/Post";
            string url = "http://localhost:51396/api/clients/subscribe?date=2016-03-10&callback=http://localhost:61051/employee/Post";
            string requestHeader = ConfigurationManager.AppSettings["ServiceRequestHeader"];
            string requestHeaderValue = ConfigurationManager.AppSettings["ServiceRequestHeaderValue"];

            HttpResponseMessage response = await url.WithHeader(requestHeader, requestHeaderValue).GetAsync();
            return response;
        }



    }
}