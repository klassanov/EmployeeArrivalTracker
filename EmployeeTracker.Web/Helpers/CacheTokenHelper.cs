using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using EmployeeTracker.Domain.Model;

namespace EmployeeTracker.Web.Helpers
{
    public class CacheTokenHelper : ITokenHelper
    {
        private Cache cache;

        public CacheTokenHelper(Cache contextBase)
        {
            this.cache = contextBase;
        }

        public bool CheckToken(string tokenValue)
        {
            throw new NotImplementedException();
        }

        public void StoreToken(SubscriptionToken subscriptionToken)
        {
            throw new NotImplementedException();
        }
    }
}