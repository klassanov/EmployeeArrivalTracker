using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EmployeeTracker.DataAccess.Interfaces;
using EmployeeTracker.Domain.Model;
using EmployeeTracker.Web.Controllers;
using EmployeeTracker.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EmployeeTracker.Tests
{
    [TestClass]
    public class EmployeeTest
    {
        private Mock<IEmployeeTrackerRepository> repositoryMock;
        private Mock<ITokenHelper> tokenHelperMock;

        private int employeesNumber;
        private DateTime startDateTime;
        private List<EmployeeArrival> employeeArrivals;

        public EmployeeTest()
        {
            Initialize();          
        }

        private void Initialize()
        {
            startDateTime = DateTime.Now;
            employeesNumber = 10;
            employeeArrivals = new List<EmployeeArrival>();
            for (int i = 0; i < employeesNumber; i++)
            {
                employeeArrivals.Add(new EmployeeArrival {Id=i, EmployeeId=i, When=startDateTime.AddMinutes(1) });
            }

            repositoryMock = new Mock<IEmployeeTrackerRepository>();
            repositoryMock.Setup(m => m.GetAll<EmployeeArrival>(null)).Returns(employeeArrivals);

            tokenHelperMock = new Mock<ITokenHelper>();
            tokenHelperMock.Setup(m => m.CheckToken(It.IsAny<string>())).Returns(true);
        }


        [TestMethod]
        public void TestOfTheTest()
        {
            Assert.AreEqual(1, 1);
        }

      
        [TestMethod]
        public void Arrivals_Contains_All_EmployeeArrivals()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(repositoryMock.Object, tokenHelperMock.Object);

            //Act
            ViewResult viewResult=controller.Arrivals();
            IEnumerable<EmployeeArrival> viewModel = viewResult.Model as IEnumerable<EmployeeArrival>;

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(viewModel.Count(), employeesNumber);
            Assert.AreEqual(viewModel.ElementAt(0).EmployeeId, 0);
            Assert.AreEqual(viewModel.Last().EmployeeId, employeesNumber - 1);
        }
       
    }
}
