using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kennels.Controllers;
using System.Collections.Generic;

namespace Kennels.Tests.Controllers
{
    [TestClass]
    public class KennelControllerTest
    {
        //Verifys that getRatingList() returns a list of ints   
        [TestMethod]
        public void getRatingListReturnsList()
        {
            var controller = new KennelsController();                       
            var result = controller.getRatingList();
            Assert.IsInstanceOfType(result, typeof(List<int>));
        }


        //Verifys that getRatingList() returns a list of 5 exact ints (1, 2, 3, 4, 5)
        [TestMethod]
        public void ratingList1to5()
        {
            var controller = new KennelsController();
            var ratList = new List<int>()
            { 1, 2, 3, 4 ,5 } ;

            var result = controller.getRatingList();

            CollectionAssert.AreEqual(ratList, result);
        }
        //Verifys that getRatingList() returns a list of strings   
        [TestMethod]
        public void getSortListReturnsList()
        {
            var controller = new KennelsController();
            var result = controller.getSortList();
            Assert.IsInstanceOfType(result, typeof(List<string>));

        }

        //Verifys that getRatingList() returns a list of 6 exact strings ("Name (A-Z)","Name (Z-A)","Price per Night (H-L)","Price per Night (L-H)", "Price per Week (H-L)","Price per Week (L-H)")
        [TestMethod]
        public void sortListTest()
        {
            var controller = new KennelsController();
            var sortList = new List<string>()
            {
                "Name (A-Z)",
                "Name (Z-A)",
                "Price per Night (H-L)",
                "Price per Night (L-H)",
                "Price per Week (H-L)",
                "Price per Week (L-H)"
            };

            var result = controller.getSortList();
            CollectionAssert.AreEqual(sortList, result);
        }

        
    }
}
