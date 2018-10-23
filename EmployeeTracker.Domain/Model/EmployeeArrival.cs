using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracker.Domain.Model
{
    public class EmployeeArrival
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime When { get; set; }

        [ForeignKey("RequestId")]
        public EmployeeArrivalPostRequest Request { get; set; }

        public int RequestId { get; set; }

    }
}
