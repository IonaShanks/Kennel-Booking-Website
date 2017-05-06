using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kennels.Controllers;
using System.Web.Mvc;

namespace Kennels.Tests.Controllers
{
    [TestClass]
    public class ManageControllerTest
    {
        [TestMethod]
        public void ChangePasswordGetTest()
        {
            var controller = new ManageController();
            var result = controller.ChangePassword() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddPhoneNumberGetTest()
        {
            var controller = new ManageController();
            var result = controller.AddPhoneNumber() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
