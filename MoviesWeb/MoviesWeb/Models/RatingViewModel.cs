using System.ComponentModel.DataAnnotations;

namespace MoviesWeb.Models
{
    public class RatingViewModel
    {
        public double AverageRating { get; set; }
        public int Count5Star { get; set; }
        public int Count4Star { get; set; }
        public int Count3Star { get; set; }
        public int Count2Star { get; set; }
        public int Count1Star { get; set; }
        public int TotalRatings { get; set; }
        public List<Comment> Comments { get; set; }
        public int? ID { get; set; }
        public int MovieID { get; set; }

        public int UserID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } =
        DateTime.Now;

        [Range(0, 10)]
        public int Rating { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }


        public string? MovieName { get; set; }

        public string? UserName { get; set; }
    }
}
