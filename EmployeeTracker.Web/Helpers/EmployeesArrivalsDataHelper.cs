using EmployeeTracker.Domain.Model;
using System.Collections.Generic;

namespace EmployeeTracker.Web.Helpers
{
    public class EmployeesArrivalsDataHelper
    {
        public void HandleEmployeesArrivalsData(string tokenValue, IEnumerable<EmployeeArrival> arrivals, ITokenHelper tokenHelper)
        {
            if (!string.IsNullOrEmpty(tokenValue) && tokenHelper.CheckToken(tokenValue))
            {
                if (arrivals != null)
                {
                    foreach (EmployeeArrival arrival in arrivals)
                    {

                    }
                }
            }
        }
    }
}