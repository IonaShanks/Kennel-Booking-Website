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

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void ContactEmailTest()
        {
            var controller = new HomeController();

            var vm = new ContactViewModel()
            {
                Message = "Message",
                EmailFrom = "email",
                Name = "Name"
            };                 

            var result = controller.Contact(vm) as ViewResult;
            Assert.AreEqual("Thank you for getting in contact, your message has been sent!", result.ViewBag.Success);
        }
    }
}
