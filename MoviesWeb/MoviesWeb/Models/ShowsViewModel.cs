namespace MoviesWeb.Models
{
    public class ShowsViewModel
    {
        public IEnumerable<Show> Shows { get; set; }
        public bool IsAdmin { get; set; }
    }
}
