using System;
using System.ComponentModel.DataAnnotations;

namespace Kennels.Models
{

    public class TotalRating
    {

        public int TotalRatings { get; set; }
        public int TotalRaters { get; set; }

        private double avgRating;
        public double AverageRating { get; set; }

        public double calcAvgRating(double totalRatings, double totalRaters)
        {
            return avgRating = totalRatings / totalRaters;
        }

        [Key]
        public String KennelID { get; set; }
        public virtual Kennel Kennel { get; set; }


    }
}