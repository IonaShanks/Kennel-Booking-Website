using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

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
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

}