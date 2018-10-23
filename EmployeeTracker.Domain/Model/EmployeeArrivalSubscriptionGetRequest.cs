using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracker.Domain.Model
{
    public class EmployeeArrivalSubscriptionGetRequest
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateParameter { get; set; }
        public string CallbackUrlParameter { get; set; }
        public string SubscriptionTokenValue { get; set; }
        public DateTime? SubscriptionTokenExpiryDate { get; set; }
        public int ResponseStatusCode { get; set; }
        public DateTime SendDateTime { get; set; }

        public EmployeeArrivalSubscriptionGetRequest()
        {
            SendDateTime = DateTime.Now;
        }
    }
}
