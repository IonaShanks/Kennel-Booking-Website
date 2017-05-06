using Kennels.Controllers;
using Kennels.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Kennels.Tests.Controllers
{

    [TestClass]
    public class RatingContollerTest
    {

        //public static HttpContextBase FakeHttpContext(MockRepository mocks, bool isAuthenticated)
        //{
        //    var context = mocks.StrictMock<HttpContextBase>();
        //    var request = mocks.StrictMock<HttpRequestBase>();
        //    var response = mocks.StrictMock<HttpResponseBase>();
        //    var session = mocks.StrictMock<HttpSessionStateBase>();
        //    var server = mocks.StrictMock<HttpServerUtilityBase>();
        //    var cachePolicy = mocks.Stub<HttpCachePolicyBase>();
        //    var user = mocks.StrictMock<IPrincipal>();
        //    var identity = mocks.StrictMock<IIdentity>();
        //    var itemDictionary = new Dictionary<object, object>();

        //    identity.Expect(id => id.IsAuthenticated).Return(isAuthenticated);
        //    user.Expect(u => u.Identity).Return(identity).Repeat.Any();

        //    context.Expect(c => c.User).PropertyBehavior();
        //    context.User = user;
        //    context.Expect(ctx => ctx.Items).Return(itemDictionary).Repeat.Any();
        //    context.Expect(ctx => ctx.Request).Return(request).Repeat.Any();
        //    context.Expect(ctx => ctx.Response).Return(response).Repeat.Any();
        //    context.Expect(ctx => ctx.Session).Return(session).Repeat.Any();
        //    context.Expect(ctx => ctx.Server).Return(server).Repeat.Any();

        //    response.Expect(r => r.Cache).Return(cachePolicy).Repeat.Any();
        //    response.Expect(r => r.StatusCode).PropertyBehavior();

        //    return context;
        //}


        //RatingsController CreateRatingsController()
        //{
        //    var testData = FakeRatingData.CreateTestRatings();
        //    var repository = new FakeRatingRepository(testData);

        //    var nerdIdentity = FakeIdentity.CreateIdentity("SomeUser");

        //    return new RatingsController(repository, nerdIdentity);
        //}

        //RatingsController CreateRatingsControllerAs(string userName)
        //{
        //    var mock = new Mock<ControllerContext>();
        //    mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(userName);
        //    mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

        //    var controller = CreateRatingsController();
        //    controller.ControllerContext = mock.Object;

        //    return controller;
        //}


        [TestMethod]
        public void Verify_Create_Rating_Has_Authorize_Attribute()
        {
            var controller = new RatingsController();
            var type = controller.GetType();
            var methodInfo = type.GetMethod("Create", new Type[] { typeof(Rating) });
            var attributes = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            Assert.IsTrue(attributes.Any(), "No AuthorizeAttribute found on ChangePassword(ChangePasswordModel model) method");
        }

        [TestMethod]
        public void CreateTest()
        {
            var controller = new RatingsController();
        }

        //[TestMethod]
        //public void Index_Returns_ActionResult()
        //{
        //    // Arrange
        //    //id = "DIT123";
        //    //var controller = new UserController();
        //    //var mock = new Mock<ControllerContext>();
        //    //mock.SetupGet(x => x.HttpContext.User.Identity.Name).Returns("SOMEUSER");
        //    //mock.SetupGet(x => x.HttpContext.Request.IsAuthenticated).Returns(true);
        //    //controller.ControllerContext = mock.Object;
        //    RatingsController controller = new RatingsController();

        //    // Act
        //    var actual = controller.KennelRatings("DIT123");
        //    //ViewResult result = controller.Index() as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(actual);
        //}


        [TestMethod]
        public void TestDetailsView()
        {
            var controller = new RatingsController();
            var result = controller.Details(22) as Task<ActionResult>;
            
            Assert.IsNotNull(result);
            
            //Assert.IsTrue(HttpStatusCodeResult.Equals(400, result.Result));
        }



[TestMethod]
        public void TestDetailsViewNotFound()
        {
        var controller = new RatingsController();            
            var result = controller.Details(null) ;            
            Assert.AreEqual(HttpStatusCode.BadRequest, result);
            Assert.IsInstanceOfType(result, typeof(HttpStatusCode));

        }
        [TestMethod]
        public void BookingTestDetailsViewUnauth()
        {
            var controller = new BookingsController();
            var result = controller.Details(null);
            Assert.AreEqual(HttpStatusCode.BadRequest, result);
            //Assert.IsInstanceOfType(result, typeof(HttpStatusCode));

        }

        [TestMethod]
        public void TestKennelRatingsView()
        {

            var controller = new RatingsController();

            var result = controller.KennelRatings("DIT123");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexAccess()
        {
            var controller = new RatingsController();
            var result = controller.Index();


        }

        //[TestMethod]
        //public void test_validation()
        //{
        //    var sut = new Rating()
        //    {
        //        //KennelID = "12",
        //        //RatingDate = DateTime.Now,
        //        //Ratings = 5,
        //        //RatingID = 5455
        //    };

        //    var context = new ValidationContext(sut, null, null);
        //    var results = new List<ValidationResult>();
        //    TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(Rating), typeof(Rating)), typeof(Rating));

        //    var isModelStateValid = Validator.TryValidateObject(sut, context, results, true);

        //    // Assert here
        //    Assert.IsTrue(isModelStateValid);
        //}




        //[TestMethod]
        //public void AuthenticatedNotIsUserRole_Should_RedirectToLogin()
        //{
        //    // Arrange
        //    var mocks = new MockRepository();
        //    var controller = new RatingsController();
        //    var httpContext = FakeHttpContext(mocks, true);
        //    controller.ControllerContext = new ControllerContext
        //    {
        //        Controller = controller,
        //        RequestContext = new RequestContext(httpContext, new RouteData())
        //    };

        //    httpContext.User.Expect(u => u.IsInRole("User")).Return(false);
        //    mocks.ReplayAll();

        //    // Act
        //    var result = controller.ActionInvoker.InvokeAction(controller.ControllerContext, "Index");
        //    var statusCode = httpContext.Response.StatusCode;

        //    // Assert
        //    Assert.IsTrue(result, "Invoker Result");
        //    Assert.AreEqual(401, statusCode, "Status Code");
        //    mocks.VerifyAll();
        //}


        //RatingsController CreateRatingsControllerAs(string userName)
        //{

        //    var mock = new Mock<ControllerContext>();
        //    mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(userName);
        //    mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

        //    var controller = new CreateRatingsController();
        //    controller.ControllerContext = mock.Object;

        //    return controller;
        //}

        //[TestMethod]
        //public void EditAction_Should_Return_EditView_When_ValidOwner()
        //{

        //    // Arrange
        //    var controller = CreateRatingsControllerAs("SomeUser");

        //    // Act
        //    var result = controller.Edit(1) as ViewResult;

        //    // Assert
        //    Assert.IsInstanceOfType(result.ViewData.Model, typeof(RatingFormViewModel));
        //}

    }
}
