namespace MoviesWeb.Models
{
    public class Movie
    {   public int? Amount { get; set; }
        public DateTime Release_date { get; set; }
        public int? Duration { get; set; }
        public string? Name { get; set; }
        public int Id { get; set; }
        public byte[]? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public int? Genre { get; set; }
        public string? Naziv {  get; set; }
        //public bool? Administrator { get; set; }
        //public int? Admin { get; set; }
        //public string? AdminName { get; set; }
        public bool Status { get; set; }
    }
}
