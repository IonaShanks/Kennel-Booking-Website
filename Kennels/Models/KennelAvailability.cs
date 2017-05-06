using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kennels.Models
{
    public class KennelAvailability
    {
        [Key, Column(Order = 1)]
        public DateTime BookingDate { get; set; }
        public int Availability { get; set; }
        public bool Full { get; set; }
        
        [Key, Column(Order = 0)]
        public String KennelID { get; set; }
        public virtual Kennel Kennel { get; set; }
    }
}