using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Kennels.Models
{
    public class KennelsContext : IdentityDbContext<ApplicationUser>
    {
        public KennelsContext() : base("Kennels")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<KennelsContext>());
        }

        public static KennelsContext Create()
        {
            return new KennelsContext();
        }

        public DbSet<Kennel> Kennel { get; set; }
        public DbSet<KennelAvailability> KennelAvailability { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<TotalRating> TotalRating { get; set; }
        public DbSet<Rating> Rating { get; set; }
        
    }
}