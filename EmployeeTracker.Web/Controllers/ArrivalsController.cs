using EmployeeTracker.DataAccess.Interfaces;
using EmployeeTracker.Domain.Model;
using Newtonsoft.Json.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EmployeeTracker.Web.Controllers
{

    public class ArrivalsController : ApiController
    {
        public IArrivalsRepository repository { get; set; }

        [HttpGet]
        public IHttpActionResult Test()
        {
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]IEnumerable<EmployeeArrival> arrivals)
        {
            return Ok();
        }

    }
}
