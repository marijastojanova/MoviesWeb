namespace MoviesWeb.Models
{
    public class Payment
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public int SecurityCode { get; set; }
        public string City {  get; set; }
        public string ZipCode { get; set; }
    }
}
