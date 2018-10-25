﻿using EmployeeTracker.DataAccess.Interfaces;
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

        public EmployeeController(IArrivalsRepository repository, ITokenHelper tokenHelper)
        {
            this.repository = repository;
            this.tokenHelper = tokenHelper;
        }

        public ViewResult Subscribe()
        {
            return View();
        }

        [HttpPost]
        public async Task<ViewResult> Subscribe(EmployeeArrivalSubscriptionGetRequest subscriptionRequest)
        {
            if(ModelState.IsValid)
            {
                string url = CreateUrlString(subscriptionRequest.DateParameter);
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
                    ViewData[Constants.MESSAGE_SUCCESS_KEY] = Resources.msg_subscription_successful;
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

        public ViewResult Arrivals()
        {
            IEnumerable<EmployeeArrival> arrivalsList = repository.GetAll<EmployeeArrival>();
            return View(arrivalsList);
        }

        public ViewResult ArrivalsById(int employeeId)
        {
            IEnumerable<EmployeeArrival> arrivalsList = repository.GetAll<EmployeeArrival>(x=>x.EmployeeId==employeeId);
            return View("Arrivals",arrivalsList);
        }

        public ViewResult PostRequests()
        {
            IEnumerable<EmployeeArrivalPostRequest> postRequests = repository.GetAll<EmployeeArrivalPostRequest>();
            return View(postRequests);
        }

        public ViewResult SubscriptionsHistory()
        {
            IEnumerable<EmployeeArrivalSubscriptionGetRequest> subscriptions=repository.GetAll<EmployeeArrivalSubscriptionGetRequest>();
            return View(subscriptions);
        }

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

        private async Task<HttpResponseMessage> GetServiceResponseMessage(string url)
        {
            string requestHeader = ConfigurationManager.AppSettings["ServiceRequestHeader"];
            string requestHeaderValue = ConfigurationManager.AppSettings["ServiceRequestHeaderValue"];
            HttpResponseMessage response = await url.WithHeader(requestHeader, requestHeaderValue).GetAsync();
            //HttpResponseMessage response = await url.GetAsync();
            return response;
        }

        private string CreateUrlString(DateTime dateParam)
        {
            return string.Format("{0}?{1}={2}&{3}={4}",
              ConfigurationManager.AppSettings["remoteServiceEndpointUrl"],
              Constants.DATE,
              dateParam.Date.ToString(ConfigurationManager.AppSettings["subscriptionRequestDateFormat"]),
              Constants.CALLBACK,
              ConfigurationManager.AppSettings["applicationServiceCallbackUrl"]);
        }


    }
}