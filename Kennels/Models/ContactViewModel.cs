using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kennels.Models
{
    //View Model for the contact page
    public class ContactViewModel
    {
        [Required, StringLength(30, MinimumLength = 4, ErrorMessage = "Must be between 4 and 30 characters.")]
        public string Name { get; set; }
        [Required, EmailAddress, Display(Name="Email")]
        public string EmailFrom { get; set; }        
        [Required]
        public string Message { get; set; }
    }
}