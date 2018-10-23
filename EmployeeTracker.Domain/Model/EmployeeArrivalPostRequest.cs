using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracker.Domain.Model
{
    public class EmployeeArrivalPostRequest
    {
        [Key]
        public int Id { get; set; }
        public string TokenValue { get; set; }
        public bool IsValid { get; set; }
        public DateTime ReceiveDateTime { get; set; }
    }
}
