using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models
{
    public class CommentShow
    {
        public int? ID { get; set; }

        public int? Show_Id { get; set; }

        public int? User_Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0, 10)]
        public int Rating { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }


        public string? ShowName { get; set; }

        public string? UserName { get; set; }
    }
}
