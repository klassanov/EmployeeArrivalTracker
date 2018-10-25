using EmployeeTracker.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EmployeeTracker.Web.Helpers
{
    public static class StringHelper
    {
        public static string CreateWebSericeSubscriptionRequestUrl(DateTime dateParam)
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