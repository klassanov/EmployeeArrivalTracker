using EmployeeTracker.DataAccess.Interfaces;
using EmployeeTracker.Domain;
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

        /// <summary>
        /// Constructor with automatically injected parameters
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="tokenHelper"></param>
        public EmployeeController(IArrivalsRepository repository, ITokenHelper tokenHelper)
        {
            this.repository = repository;
            this.tokenHelper = tokenHelper;
        }

        /// <summary>
        /// Employee/Subscribe gets the subscription view for making a new subscription
        /// </summary>
        /// <returns></returns>
        public ViewResult Subscribe()
        {
            return View();
        }

        /// <summary>
        /// POST Emplyee/Subscribe
        /// <para>Makes a subscripton request to the remote web service with the DateParameter property specified in subscriptionRequest</para>
        /// <para>Returns the subscription view with a feedback message for the user</para>
        /// </summary>
        /// <param name="subscriptionRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ViewResult> Subscribe(EmployeeArrivalSubscriptionGetRequest subscriptionRequest)
        {
            if (ModelState.IsValid)
            {
                string url = StringHelper.CreateWebSericeSubscriptionRequestUrl(subscriptionRequest.DateParameter);
                subscriptionRequest.CallbackUrlParameter = url;
                HttpResponseMessage response = await GetServiceResponseMessage(url);
                subscriptionRequest.ResponseStatusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    SubscriptionToken subscriptionToken = JsonConvert.DeserializeObject<SubscriptionToken>(content);
                    tokenHelper.StoreToken(subscriptionToken);
                    subscriptionRequest.SubscriptionTokenValue = subscriptionToken.Token;
                    subscriptionRequest.SubscriptionTokenExpiryDate = subscriptionToken.Expires;
                    ViewData[Constants.MESSAGE_SUCCESS_KEY] = string.Format(Resources.msg_subscription_successful, subscriptionRequest.DateParameter.ToString("dd-MMMM-yyyy"));
                }
                else
                {
                    ViewData[Constants.MESSAGE_ERROR_KEY] = string.Format("{0} Response: {1} {2}",
                        Resources.msg_subscription_not_successful, response.StatusCode, response.ReasonPhrase);
                }
                repository.WriteSubscriptionRequest(subscriptionRequest);
            }
            else
            {
                ViewData[Constants.MESSAGE_ERROR_KEY] = Resources.msg_subscription_not_successful;
            }
            return View();
        }

        /// <summary>
        /// Employee/Arrivals gets the employee arrivals view populated with all the entries
        /// </summary>
        /// <returns></returns>
        public ViewResult Arrivals()
        {
            IEnumerable<EmployeeArrival> arrivalsList = repository.GetAll<EmployeeArrival>();
            return View(arrivalsList);
        }

        /// <summary>
        /// Employee/ArrivalsById Gets the employees arrivals view filtered by employeeId
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public ViewResult ArrivalsById(int employeeId)
        {
            IEnumerable<EmployeeArrival> arrivalsList = repository.GetAll<EmployeeArrival>(x => x.EmployeeId == employeeId);
            return View("Arrivals", arrivalsList);
        }

        /// <summary>
        /// Employee/PostRequests gets the view with all the post requests that have been received 
        /// </summary>
        /// <returns></returns>
        public ViewResult PostRequests()
        {
            IEnumerable<EmployeeArrivalPostRequest> postRequests = repository.GetAll<EmployeeArrivalPostRequest>();
            return View(postRequests);
        }

        /// <summary>
        /// Employee/SubscriptionsHistory gets the view with all the subscriptions that have been made
        /// </summary>
        /// <returns></returns>
        public ViewResult SubscriptionsHistory()
        {
            IEnumerable<EmployeeArrivalSubscriptionGetRequest> subscriptions = repository.GetAll<EmployeeArrivalSubscriptionGetRequest>();
            return View(subscriptions);
        }

        /// <summary>
        /// POST Employee/Post the web service callback url. 
        /// <para>Validates the received token and writes to the DB the information in the POST request</para>
        /// </summary>
        /// <param name="arrivals"></param>
        [HttpPost]
        public void Post(IEnumerable<EmployeeArrival> arrivals)
        {
            string xFourthTokenHeader = ConfigurationManager.AppSettings["xFourthTokenHeader"];
            string tokenValue = Request.Headers[xFourthTokenHeader];

            EmployeeArrivalPostRequest postRequest = new EmployeeArrivalPostRequest
            {
                ReceiveDateTime = DateTime.Now,
                IsValid = tokenHelper.CheckToken(tokenValue),
                TokenValue = tokenValue
            };

            if (!postRequest.IsValid)
            {
                arrivals = null;
            }
            repository.WriteArrivalPostRequest(postRequest, arrivals);
        }

        /// <summary>
        /// Makes the request to url and returns the response
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetServiceResponseMessage(string url)
        {
            string requestHeader = ConfigurationManager.AppSettings["ServiceRequestHeader"];
            string requestHeaderValue = ConfigurationManager.AppSettings["ServiceRequestHeaderValue"];
            HttpResponseMessage response = await url.WithHeader(requestHeader, requestHeaderValue).GetAsync();
            return response;
        }
    }
}