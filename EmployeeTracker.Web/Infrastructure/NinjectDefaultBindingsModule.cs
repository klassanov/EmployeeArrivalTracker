﻿using EmployeeTracker.DataAccess.Interfaces;
using EmployeeTracker.DataAccess.Repositories;
using EmployeeTracker.Web.Helpers;
using Ninject.Modules;

namespace EmployeeTracker.Web.Infrastructure
{
    public class NinjectDefaultBindingsModule:NinjectModule
    {
        public override void Load()
        {
            Bind<IEmployeeTrackerRepository>().To<DefaultEmployeeTrackerRepository>();
            //Bind<ITokenHelper>().To<SimpleTokenHelper>().InSingletonScope();
            Bind<ITokenHelper>().To<CacheTokenHelper>().InSingletonScope();
        }
    }
}