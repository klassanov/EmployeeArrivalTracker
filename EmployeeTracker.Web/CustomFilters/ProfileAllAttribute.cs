using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracker.Web.CustomFilters
{
    public class ProfileAllAttribute:ActionFilterAttribute
    {
        private static ILog Logger = LogManager.GetLogger(typeof(ProfileAllAttribute).Name);

        private Stopwatch timer;

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            timer.Stop();
            Logger.DebugFormat("Controller: {0}; Action:{1}; Total elapsed time: {2}s",
                filterContext.Controller, filterContext.RouteData.Values["action"].ToString(), timer.Elapsed.Seconds);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            timer = Stopwatch.StartNew();
        }
    }
}