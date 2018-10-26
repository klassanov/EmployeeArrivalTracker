using EmployeeTracker.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracker.Web.Helpers
{
    public class SimpleTokenHelper : ITokenHelper
    {
        private SubscriptionToken subscriptionToken;

        public bool CheckToken(string tokenValue)
        {
            return subscriptionToken != null && subscriptionToken.Token.Equals(tokenValue);
        }

        public SubscriptionToken GetToken()
        {
            return subscriptionToken;
        }

        public void StoreToken(SubscriptionToken subscriptionToken)
        {
            this.subscriptionToken = subscriptionToken;
        }
    }
}