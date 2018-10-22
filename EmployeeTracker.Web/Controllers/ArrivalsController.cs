using EmployeeTracker.Domain.Model;
using Newtonsoft.Json.Linq;
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
       

        [HttpPost]
        public IHttpActionResult Post([FromBody]IEnumerable<EmployeeArrival> arrivals)
        {
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
