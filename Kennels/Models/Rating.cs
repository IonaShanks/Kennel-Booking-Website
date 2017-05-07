using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kennels.Models
{
    public class Rating
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RatingID { get; set; }
        [Required, Range(1, 5, ErrorMessage = "{0} must be between 1 and 5."), Display(Name = "Star Rating")]
        public int Ratings { get; set; }
        [MaxLength(250, ErrorMessage = "{0} must be less than 250 characters.")]
        public String Comment { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Review Date")]
        public DateTime RatingDate { get; set; }

        public String KennelID { get; set; }
        public virtual Kennel Kennel { get; set; }

        public ApplicationUser User { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}