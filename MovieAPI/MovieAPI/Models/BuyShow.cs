namespace MovieAPI.Models
{
    public class BuyShow
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public int? Show_Id { get; set; }
        public string? ShowName { get; set; }
        public string? Name { get; set; }
        public int? Amount { get; set; }
        public DateTime? Release_date { get; set; }
        public int? Duration { get; set; }
        public byte[]? ImageUrlShow { get; set; }
        public byte[]? ImageUrlMovie { get; set; }
        public int? Genre { get; set; }
        public string? Naziv { get; set; }
        public string? TrailerLinkShow { get; set; }
        public string? TrailerLinkMovie { get; set; }
        public string? MovieName { get; set; }
        public int? Movie_Id { get; set; }
    }
}
