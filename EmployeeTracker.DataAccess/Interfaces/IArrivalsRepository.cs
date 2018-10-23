using EmployeeTracker.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracker.DataAccess.Interfaces
{
    public interface IArrivalsRepository
    {
        void WriteSubscriptionRequest(EmployeeArrivalSubscriptionGetRequest request);
        void WriteArrivalPostRequest(EmployeeArrivalPostRequest request, IEnumerable<EmployeeArrival> employeeArrivals);
        IEnumerable<EmployeeArrival> GetEmployeeArrivals();
    }
}
