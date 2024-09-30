using Microsoft.AspNetCore.Mvc;

namespace MovieAPI.Models
{
    public class Movie
    {
        public int? Amount { get; set; }
        public DateTime? Release_date { get; set; }
        public int? Duration { get; set; }

        public string Name { get; set; }
        public int? Id { get; set; }
        public byte[]? ImageUrl { get; set; }
        public int Genre { get; set; }
        public string? Naziv { get; set; }
        public bool Status { get; set; }
        public string? Trailer_Link { get; set; }
        public bool? Administrator { get; set; }


        //public string? ImageBase64
        //{
        //    get
        //    {
        //        if (ImageUrl != null && ImageUrl.Length > 0)
        //        {
                    //return "data:image/jpeg;base64," + Convert.ToBase64String(ImageUrl);
                    // Change "image/jpeg" to the appropriate MIME type if your image format is different.
        //        }
        //        return string.Empty;
        //    }

        //}

        //public DateTime? Datum_od { get; set; }

        //public DateTime? Datum_do { get; set; }

        //public IFormFile? PosterFile { get; set; }
        //public string? Description { get; set; }
    }

}