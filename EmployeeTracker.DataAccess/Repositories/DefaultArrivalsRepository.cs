using EmployeeTracker.DataAccess.DbContext;
using EmployeeTracker.DataAccess.Interfaces;
using EmployeeTracker.Domain.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracker.DataAccess.Repositories
{
    public class DefaultArrivalsRepository : IArrivalsRepository
    {
        private static ILog logger = LogManager.GetLogger(typeof(DefaultArrivalsRepository));

        public IEnumerable<EmployeeArrival> GetEmployeeArrivals()
        {
            IEnumerable<EmployeeArrival> employeeArrivals = null;
            using (var db = new EmployeeArrivalsContext())
            {
                employeeArrivals = db.EmployeeArrivals.OrderBy(x => x.When).ThenBy(x => x.EmployeeId).ToList();
            }
            return employeeArrivals;
        }

        public void WriteArrivalPostRequest(EmployeeArrivalPostRequest request, IEnumerable<EmployeeArrival> employeeArrivals)
        {
            using (var db = new EmployeeArrivalsContext())
            {
                db.EmployeeArrivalsPostRequest.Add(request);
                if(employeeArrivals!=null)
                {
                    employeeArrivals.ToList().ForEach(x => x.Request = request);
                    db.EmployeeArrivals.AddRange(employeeArrivals);
                }
                db.SaveChanges();
            }
        }

        public void WriteSubscriptionRequest(EmployeeArrivalSubscriptionGetRequest request)
        {
            using (var db = new EmployeeArrivalsContext())
            {
                db.EmployeeArrivalSubscriptionGetRequests.Add(request);
                db.SaveChanges();
            }
            logger.DebugFormat("Saved EmployeeArrivalSubscriptionGetRequest with id {0}", request.Id);
        }
    }
}
