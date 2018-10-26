using EmployeeTracker.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracker.Web.Helpers
{
    public interface ITokenHelper
    {
        SubscriptionToken GetToken();
        bool CheckToken(string tokenValue);
        void StoreToken(SubscriptionToken subscriptionToken);
    }
}
