using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kennels.Controllers;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Kennels.Models;

namespace Kennels.Tests.Controllers
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDetailsView()
        {
            var controller = new KennelsController();
            var result = controller.Details("Ken123") as Task<ActionResult>;
            Assert.AreEqual("kennels", result);


            //var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            //Assert.AreEqual("Home", redirectToActionResult.ControllerName);
            //Assert.AreEqual("Index", redirectToActionResult.ActionName);

        }


        [TestMethod]
            public void Verify_ChangePassword_Method_Is_Decorated_With_Authorize_Attribute()
            {
                var controller = new ManageController();
                var type = controller.GetType();
                var methodInfo = type.GetMethod("ChangePassword", new Type[] { typeof(ChangePasswordViewModel) });
                var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                Assert.IsTrue(attributes.Any(), "No AuthorizeAttribute found on ChangePassword(ChangePasswordModel model) method");
            }

       

    }
}
