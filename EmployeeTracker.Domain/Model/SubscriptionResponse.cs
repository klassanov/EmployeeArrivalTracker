using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracker.Domain.Model
{
    public class SubscriptionResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
