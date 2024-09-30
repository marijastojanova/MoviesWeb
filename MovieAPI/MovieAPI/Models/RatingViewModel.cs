﻿using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models
{
    public class RatingViewModel
    {

        public int Id { get; set; }
        public double AverageRating { get; set; }
        public int Count5Star { get; set; }
        public int Count4Star { get; set; }
        public int Count3Star { get; set; }
        public int Count2Star { get; set; }
        public int Count1Star { get; set; }
        public int TotalRatings { get; set; }
        public int MovieID { get; set; }

        public int UserID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0, 10)]
        public int Rating { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }


        public string MovieName { get; set; }

        public string UserName { get; set; }
    }
}
