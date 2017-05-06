using System;
using Kennels.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kennels.Tests.Models
{
    [TestClass]
    public class TotalRatingTest
    {
        [TestMethod]        
        public void CalcAvgRatingTestPass()
        {
            var model = new TotalRating();
            double totalRatings = 15;
            double totalRaters = 4;

            double result = model.calcAvgRating(totalRatings, totalRaters);

            Assert.AreEqual(3.75, result);
        }
        [TestMethod]
        public void CalcAvgRatingTestFail()
        {
            var model = new TotalRating();
            double totalRatings = 15;
            double totalRaters = 4;

            double result = model.calcAvgRating(totalRatings, totalRaters);

            Assert.AreNotEqual(22, result);
        }
    }
}
