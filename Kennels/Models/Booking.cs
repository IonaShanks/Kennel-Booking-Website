using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kennels.Models
{
    //Validation to check that the date is not in the past.
    public class FutureDateAttribute : ValidationAttribute
    {        
        public override bool IsValid(object value)
        {
            DateTime date = Convert.ToDateTime(value);
            return date >= DateTime.Now;
        }  
    }

    //Validation to check that the start date is before the end date.
    public class DateAfterAttribute : ValidationAttribute
    {
        public DateAfterAttribute(string dateToCompare)
        {
            DateToCompare = dateToCompare;
        }
        private string DateToCompare { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime endDate = (DateTime)value;
            DateTime startDate = (DateTime)validationContext.ObjectType.GetProperty(DateToCompare).GetValue(validationContext.ObjectInstance, null);

            if (startDate > endDate)
            {
                return new ValidationResult("Stay must be at least one night");                
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    public class Booking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingID { get; set; }
        [Required, DataType(DataType.Date), FutureDate(ErrorMessage = "Date cannot be in the past"), Display(Name ="From")]
        public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Date), DateAfter("StartDate")/*(ErrorMessage = "Stay must be at least one night")*/, Display(Name ="To")]        
        public DateTime EndDate { get; set; }
        
        //Calculates the total nights based on the start and end date entered by the user. 
        public int TotalNights { get; set; }

        public int CalcTotalNights(DateTime startDate, DateTime endDate)
        {
            int Tnights = (endDate - startDate).Days;
            return Tnights;

        }        
        public double Price { get; set; }

        //Calculates the price by calculating how many nights remain after 7/14/21 etc (one week) and calculating the price per night
        //and then calculating the number of weeks and calculating the price per week, and adding them together to get the total price.

        public double CalcTotalPrice(double pricePerNight, double pricePerWeek, int totalNights)
        {
            const int week = 7;            
            double totalPrice = ((totalNights % week) * pricePerNight) + ((totalNights / week) * pricePerWeek);
            return totalPrice;
        }

        public String KennelID { get; set; }
        public Kennel Kennel { get; set; }

        public ApplicationUser User { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}