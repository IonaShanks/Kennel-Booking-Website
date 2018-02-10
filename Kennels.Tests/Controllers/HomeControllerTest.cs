using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kennels;
using Kennels.Controllers;
using Kennels.Models;

namespace Kennels.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
       
        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Helping you find the ideal kennel.", result.ViewBag.Message);
        }            
    }
}
