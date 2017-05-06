// <copyright file="BookingTest.cs">Copyright ©  2017</copyright>
using System;
using Kennels.Models;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kennels.Models.Tests
{
    /// <summary>This class contains parameterized unit tests for Booking</summary>
    [PexClass(typeof(Booking))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class BookingTest
    {
        /// <summary>Test stub for CalcTotalNights(DateTime, DateTime)</summary>
        [PexMethod]
        [TestMethod]
        public int CalcTotalNightsTest(
            [PexAssumeUnderTest]Booking target,
            DateTime startDate,
            DateTime endDate
        )
        {
            startDate = DateTime.Now;
            endDate = DateTime.Now.AddDays(7);

            int result = target.CalcTotalNights(startDate, endDate);
            return result;

            Assert.AreEqual(7, result);
            // TODO: add assertions to method BookingTest.CalcTotalNightsTest(Booking, DateTime, DateTime)
        }

        /// <summary>Test stub for CalcTotalPrice(Double, Double, Int32)</summary>
        [PexMethod]
        public double CalcTotalPriceTest(
            [PexAssumeUnderTest]Booking target,
            double pricePerNight,
            double pricePerWeek,
            int totalNights
        )
        {
            double result = target.CalcTotalPrice(pricePerNight, pricePerWeek, totalNights);
            return result;
            // TODO: add assertions to method BookingTest.CalcTotalPriceTest(Booking, Double, Double, Int32)
        }

        [TestMethod]
        public void CalcTotalNightsTest(           
            DateTime startDate,
            DateTime endDate
        )
        {
            var model = new BookingViewModel();
            startDate = DateTime.Now;
            endDate = DateTime.Now.AddDays(7);

            int result = model.CalcTotalNights(startDate, endDate);
            
            Assert.AreEqual(7, result);
            // TODO: add assertions to method BookingTest.CalcTotalNightsTest(Booking, DateTime, DateTime)
        }

    }
}
