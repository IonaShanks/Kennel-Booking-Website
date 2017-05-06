using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kennels.Models;

namespace Kennels.Tests.Models
{
    [TestClass]
    public class BookingTest
    {        
        [TestMethod]
        public void CalcTotalNightsTestPass()
        {
            var model = new BookingViewModel();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(7);

            int result = model.CalcTotalNights(startDate, endDate);

            Assert.AreEqual(7, result);            
        }

        [TestMethod]
        public void CalcTotalNightsTestFail()
        {
            var model = new BookingViewModel();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(7);

            int result = model.CalcTotalNights(startDate, endDate);

            Assert.AreNotEqual(17, result);

        }
        [TestMethod]
        public void CalcTotalPriceTestPass()
        {
            var model = new BookingViewModel();

            double pricePerNight = 10;
            double pricePerWeek = 60;
            int totalNights = 9;
            
            double result = model.CalcTotalPrice(pricePerNight, pricePerWeek, totalNights);

            Assert.AreEqual(80, result);
        }
        [TestMethod]
        public void CalcTotalPriceTestFail()
        {
            var model = new BookingViewModel();

            double pricePerNight = 10;
            double pricePerWeek = 60;
            int totalNights = 9;

            double result = model.CalcTotalPrice(pricePerNight, pricePerWeek, totalNights);

            Assert.AreNotEqual(180, result);
        }
    }
}
