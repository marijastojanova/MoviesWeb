namespace MoviesWeb.Models
{
    public class MoviesViewModel
    {
        public IEnumerable<Movie> Movies { get; set; }
        public bool IsAdmin { get; set; }
    }
}
