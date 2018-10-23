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
        public IHttpActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]IEnumerable<EmployeeArrival> arrivals)
        {
            //throw new Exception("Test");
            IEnumerable<string> headerValues;
            string header = ConfigurationManager.AppSettings["ServicePostRequestTokenHeader"];
            if (Request.Headers.TryGetValues(header, out headerValues))
            {
                if(headerValues.FirstOrDefault()!=null)
                {
                    string token = headerValues.FirstOrDefault();

                    //Save entities
                    if(arrivals!=null)
                    {
                        foreach (EmployeeArrival arrival in arrivals)
                        {

                        }
                    }
                }
            }


            return Ok();
        }

    }
}
