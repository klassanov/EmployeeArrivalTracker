using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using EmployeeTracker.Domain;
using EmployeeTracker.Domain.Model;

namespace EmployeeTracker.Web.Helpers
{
    public class CacheTokenHelper : ITokenHelper
    {
        private Cache cache;

        public CacheTokenHelper(Cache cache)
        {
            this.cache = cache;
        }

        public bool CheckToken(string tokenValue)
        {
            SubscriptionToken token = cache[Constants.FOURTH_TOKEN_KEY] as SubscriptionToken;
            return token != null && token.Token.Equals(tokenValue);
        }

        public void StoreToken(SubscriptionToken subscriptionToken)
        {
            cache[Constants.FOURTH_TOKEN_KEY] = subscriptionToken;
        }
    }
}