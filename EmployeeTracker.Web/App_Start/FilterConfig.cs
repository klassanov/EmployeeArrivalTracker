﻿using EmployeeTracker.Web.CustomFilters;
using System.Web;
using System.Web.Mvc;

namespace EmployeeTracker.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new LogExceptionAttribute());
            filters.Add(new ProfileAllAttribute());
        }
    }
}
