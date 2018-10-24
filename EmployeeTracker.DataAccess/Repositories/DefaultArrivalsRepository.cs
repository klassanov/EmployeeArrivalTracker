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

        public IEnumerable<T> GetAll<T>(Func<T, bool> wherePredicate=null) where T: class
        {
            IEnumerable<T> result = null;
            using (var db = new EmployeeArrivalsContext())
            {
                result=wherePredicate !=null
                    ?db.Set<T>().Where(wherePredicate).ToList()
                    :db.Set<T>().ToList();                
            }
            logger.DebugFormat("{0} {1} entities retrieved", result.Count(), typeof(T).Name);
            return result;
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
            logger.DebugFormat("Saved EmployeeArrivalPostRequest with id {0}", request.Id);
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
