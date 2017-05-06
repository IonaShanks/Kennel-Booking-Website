using System;
using Kennels.Controllers;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kennels.Controllers.Tests
{
    /// <summary>This class contains parameterized unit tests for HomeController</summary>
    [TestClass]
    [PexClass(typeof(HomeController))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class HomeControllerTest
    {

        /// <summary>Test stub for ContactEmail(String, String, String)</summary>
        [PexMethod]
        public void ContactEmailTest(
            [PexAssumeUnderTest]HomeController target,
            string Message,
            string Email,
            string Name
        )
        {
            target.ContactEmail(Message, Email, Name);
            // TODO: add assertions to method HomeControllerTest.ContactEmailTest(HomeController, String, String, String)
        }
    }
}
