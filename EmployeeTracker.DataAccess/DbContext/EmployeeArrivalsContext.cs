namespace EmployeeTracker.DataAccess.DbContext
{
    using EmployeeTracker.Domain.Model;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EmployeeArrivalsContext : DbContext
    {
        public EmployeeArrivalsContext()
            : base("name=EmployeeArrivalsContext")
        {
        }

        public DbSet<EmployeeArrival> EmployeeArrivals { get; set; }
        public DbSet<EmployeeArrivalPostRequest> EmployeeArrivalsPostRequest { get; set; }
        public DbSet<EmployeeArrivalSubscriptionGetRequest> EmployeeArrivalSubscriptionGetRequests { get; set; }

    }

}