using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kennels.Models
{    
    public enum County
    {
        Carlow, Cavan, Clare, Cork, Donegal, Dublin, Galway, Kerry, Kildare, Kilkenny, Laois, Leitrim, Limerick, Longford, Louth, Mayo, Meath, Monaghan, Offaly, Roscommon, Sligo, Tipperary, Waterford, Westmeath, Wexford, Wicklow
    }

    public class Kennel
    {
        [Key, MaxLength(6, ErrorMessage = "{0} must be between 1 and 6 characters."), Display(Name = "Kennel ID")]
        public String KennelID { get; set; }

        [Required, Display(Name = "Kennel Name")]
        public String Name { get; set; }

        [Required]
        public String Address { get; set; }

        [Required, Display(Name ="Town/City")]
        public String Town {get; set;}

        [Required]
        public County County { get; set; }   
        
        [Required, Phone, Display(Name = "Phone Number")]
        public String PhoneNumber { get; set; }

        [Required, EmailAddress, Display(Name= "Email")]
        public String Email { get; set; }

        [Required, Range(1, Int32.MaxValue, ErrorMessage = "{0} must be greater than 0.")]
        public int Capacity { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:C}"), Required, Display(Name = "Price per Night"), Range(0.01, Int32.MaxValue, ErrorMessage = "{0} must be greater than 0.")]
        public double PricePerNight { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}"), Required, Display(Name = "Price per Week"), Range(0.01, Int32.MaxValue, ErrorMessage = "{0} must be greater than 0.")]
        public double PricePerWeek { get; set; }

        [Required, Display(Name = "Maximum Nights"), Range(1, Int32.MaxValue, ErrorMessage = "{0} must be greater than 0.")]
        public int MaxDays { get; set; }

        [Required, Display(Name = "Large Dog")]
        public bool LgeDog { get; set; }

        [Required, Display(Name = "Medium Dog")]
        public bool MedDog { get; set; }

        [Required, Display(Name = "Small Dog")]
        public bool SmlDog { get; set; }

        [Required, Display(Name = "Cancellation Period (Hours)"), Range(1, Int32.MaxValue, ErrorMessage = "{0} must be greater than 0.")]
        public double CancellationPeriod { get; set; }

        [Display(Name ="Kennel Description")]
        public String Description { get; set; }

        [Required]
        public bool Grooming { get; set; }

        [Required]
        public bool Training { get; set; }

        public ApplicationUser User { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual List<KennelAvailability> KennelAvailability { get; set; }
        public virtual List<Booking> Booking { get; set; }
        public virtual List<TotalRating> TotalRating { get; set; }
        public virtual List<Rating> Rating { get; set; }

    }
}