using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kennels.Models;
using System.Web;
using System.Collections.Generic;

namespace Kennels.Tests.Models
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void KennelShouldBeValid()
        {

            //Arrange
            Kennel kennel = new Kennel()
            {
                KennelID = "UT123",
                Name = "Jim",
                Address = "2 Cool",
                County = County.Dublin,
                PhoneNumber = "12155842",
                Email = "email@email.com",
                Capacity = 1,
                PricePerNight = 10,
                PricePerWeek = 65,
                MaxDays = 5,
                Town = "Barley",
                LgeDog = false,
                MedDog = true,
                SmlDog = true,
                CancellationPeriod = 24
                //User = "7d2785cc-284a-4c38-ac08-37f47e171d02",
                
            };

            List<Kennel> kennels = new List<Kennel>();
            kennels.Add(kennel);

            // Act
            bool isValid = false;
            if (kennels != null) { isValid = true; }
            //Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void KennelShouldNotBeValid()
        {

            Kennel kennel = new Kennel()
            {
                
                Name = "Jim",
                Address = "2 Cool",
                County = County.Dublin,
                PhoneNumber = "12155842",
                Email = "email",
                Capacity = 1,
                PricePerNight = 10,
                PricePerWeek = 65,
                MaxDays = 5
                //User = "7d2785cc-284a-4c38-ac08-37f47e171d02",

            };

            // Act
            List<Kennel> kennels = new List<Kennel>();
            kennels.Add(kennel);

            // Act
            bool isValid = false;
            if (kennels != null) { isValid = true; }

            //Assert
            Assert.IsFalse(isValid);
        }


    }
}
