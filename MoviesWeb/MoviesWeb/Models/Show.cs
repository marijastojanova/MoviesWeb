namespace MoviesWeb.Models
{
    public class Show
    {
        public int? Amount { get; set; }
        public DateTime Release_date { get; set; }
        public int? Duration { get; set; }
        public string? Name { get; set; }
        public int Id { get; set; }
        public byte[]? Img_url { get; set; }
        public int? Genre { get; set; }
        public string? Naziv { get; set; }
        public bool Status { get; set; }
        //public string? Trailer_Link { get; set; }
        public IFormFile? ImageFile { get; set; }

    }
}
