using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kennels.Controllers;
using System.Collections.Generic;

namespace Kennels.Tests.Controllers
{
    [TestClass]
    public class KennelControllerTest
    {
        [TestMethod]
        public void getRatingListReturnsList()
        {
            var controller = new KennelsController();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(7);
            
            var result = controller.getRatingList();

            Assert.IsInstanceOfType(result, typeof(List<int>));
        }
    }
}
