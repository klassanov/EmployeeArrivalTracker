using EmployeeTracker.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeTracker.Web.Helpers
{
    public class SimpleTokenHelper:ITokenHelper
    {
        private Dictionary<string, SubscriptionToken> subscriptionTokensDict = new Dictionary<string, SubscriptionToken>();

        public bool CheckToken(string tokenValue)
        {
            bool isValid = true;
            if (subscriptionTokensDict.ContainsKey(tokenValue))
            {
                SubscriptionToken token = subscriptionTokensDict[tokenValue];
                if (token.Expires > DateTime.Now)
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                    subscriptionTokensDict.Remove(tokenValue);
                }
            }
            return isValid;
        }
        public void StoreToken(SubscriptionToken subscriptionToken)
        {
            if (subscriptionTokensDict.ContainsKey(subscriptionToken.Token))
            {
                subscriptionTokensDict.Remove(subscriptionToken.Token);
            }
            subscriptionTokensDict.Add(subscriptionToken.Token, subscriptionToken);
        }
    }
}