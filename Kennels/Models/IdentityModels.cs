using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Kennels.Models
{
    public enum UserType
    { KennelOwner, Customer }
    
    public class ApplicationUser : IdentityUser
    {
        [Required, Display(Name = "First Name")]
        public string Fname { get; set; }

        [Required, Display(Name = "Last Name")]
        public string Lname { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public County County { get; set; }

        [Required, Phone, Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required, Display(Name = "User Type")]
        public UserType UserType { get; set; }

        public virtual List<Kennel> Kennel { get; set; }
        public virtual List<Booking> Booking { get; set; }
        public virtual List<Rating> Rating { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
 
}