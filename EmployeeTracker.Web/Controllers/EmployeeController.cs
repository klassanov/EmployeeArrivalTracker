using EmployeeTracker.Domain;
using EmployeeTracker.Domain.Model;
using Flurl.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace EmployeeTracker.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ViewResult> List()
        {
            throw new Exception("test");
            HttpResponseMessage response = await GetServiceResponseMessage();
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                SubscriptionResponse subscriptionResponse = JsonConvert.DeserializeObject<SubscriptionResponse>(content);
            }
            else
            {

            }
            return View();
        }

        private async Task<HttpResponseMessage> GetServiceResponseMessage()
        {
            string url ="http://localhost:51396/api/clients/subscribe?date=2016-03-10&callback=http://localhost:61051/api/arrivals/Post";
            string requestHeader = ConfigurationManager.AppSettings["ServiceRequestHeader"];
            string requestHeaderValue = ConfigurationManager.AppSettings["ServiceRequestHeaderValue"];

            HttpResponseMessage response = await url.WithHeader(requestHeader, requestHeaderValue).GetAsync();
            return response;
        }



    }
}