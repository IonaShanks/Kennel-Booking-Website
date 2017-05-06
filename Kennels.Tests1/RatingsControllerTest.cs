using System.Threading.Tasks;
using Kennels.Models;
// <copyright file="RatingsControllerTest.cs">Copyright ©  2017</copyright>
using System;
using System.Web.Mvc;
using Kennels.Controllers;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kennels.Controllers.Tests
{
    /// <summary>This class contains parameterized unit tests for RatingsController</summary>
    [PexClass(typeof(RatingsController))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class RatingsControllerTest
    {
        /// <summary>Test stub for KennelRatings(String)</summary>
        [PexMethod]
        public ActionResult KennelRatingsTest([PexAssumeUnderTest]RatingsController target, string id)
        {
            ActionResult result = target.KennelRatings(id);
            return result;
            // TODO: add assertions to method RatingsControllerTest.KennelRatingsTest(RatingsController, String)
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        [TestMethod]
        public RatingsController ConstructorTest()
        {
            RatingsController target = new RatingsController();
            return target;
            // TODO: add assertions to method RatingsControllerTest.ConstructorTest()
        }

        /// <summary>Test stub for Create(String, ApplicationUser)</summary>
        [PexMethod]
        public ActionResult CreateTest(
            [PexAssumeUnderTest]RatingsController target,
            string id,
            ApplicationUser user
        )
        {
            ActionResult result = target.Create(id, user);
            return result;
            // TODO: add assertions to method RatingsControllerTest.CreateTest(RatingsController, String, ApplicationUser)
        }

        /// <summary>Test stub for Create(String, Rating)</summary>
        [PexMethod]
        public Task<ActionResult> CreateTest01(
            [PexAssumeUnderTest]RatingsController target,
            string id,
            Rating rating
        )
        {
            Task<ActionResult> result = target.Create(id, rating);
            return result;
            // TODO: add assertions to method RatingsControllerTest.CreateTest01(RatingsController, String, Rating)
        }

        /// <summary>Test stub for Delete(Nullable`1&lt;Int32&gt;)</summary>
        [PexMethod]
        public Task<ActionResult> DeleteTest([PexAssumeUnderTest]RatingsController target, int? id)
        {
            Task<ActionResult> result = target.Delete(id);
            return result;
            // TODO: add assertions to method RatingsControllerTest.DeleteTest(RatingsController, Nullable`1<Int32>)
        }

        /// <summary>Test stub for DeleteConfirmed(Int32)</summary>
        [PexMethod]
        public Task<ActionResult> DeleteConfirmedTest([PexAssumeUnderTest]RatingsController target, int id)
        {
            Task<ActionResult> result = target.DeleteConfirmed(id);
            return result;
            // TODO: add assertions to method RatingsControllerTest.DeleteConfirmedTest(RatingsController, Int32)
        }

        /// <summary>Test stub for Details(Nullable`1&lt;Int32&gt;)</summary>
        [PexMethod]
        public Task<ActionResult> DetailsTest([PexAssumeUnderTest]RatingsController target, int? id)
        {
            Task<ActionResult> result = target.Details(id);
            return result;
            // TODO: add assertions to method RatingsControllerTest.DetailsTest(RatingsController, Nullable`1<Int32>)
        }

        /// <summary>Test stub for Edit(Nullable`1&lt;Int32&gt;)</summary>
        [PexMethod]
        public Task<ActionResult> EditTest([PexAssumeUnderTest]RatingsController target, int? id)
        {
            Task<ActionResult> result = target.Edit(id);
            return result;
            // TODO: add assertions to method RatingsControllerTest.EditTest(RatingsController, Nullable`1<Int32>)
        }

        /// <summary>Test stub for Edit(Rating)</summary>
        [PexMethod]
        public Task<ActionResult> EditTest01([PexAssumeUnderTest]RatingsController target, Rating rating)
        {
            Task<ActionResult> result = target.Edit(rating);
            return result;
            // TODO: add assertions to method RatingsControllerTest.EditTest01(RatingsController, Rating)
        }

        /// <summary>Test stub for Index()</summary>
        [PexMethod]
        public ActionResult IndexTest([PexAssumeUnderTest]RatingsController target)
        {
            ActionResult result = target.Index();
            return result;
            // TODO: add assertions to method RatingsControllerTest.IndexTest(RatingsController)
        }
       
    }
}
