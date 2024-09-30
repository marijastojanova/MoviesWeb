using MovieAPI.Models;

namespace MovieAPI.Repository
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllMovies();
        Task<Movie> GetMovieById(int id);
        Task<int> InsertMovie(Movie newDto);
        Task<int> UpdateMovie(int id, Movie editDto);
        Task<int> DeleteMovie(int id);
        Task<IEnumerable<Movie>> GetMoviesByGenre(int genre);
        Task<int> CreateTicket(BuyTicket buyTicket);
        Task<int> DeleteTicket(int id);
        Task<IEnumerable<BuyTicket>> GetAllByTicket(int userId);
        Task<Movie> GetTicketById(int id);
        Task<bool> InsertAndDeleteAll(int userId);
        Task<IEnumerable<BuyShow>> GetAllByShow(int userId);
        Task<IEnumerable<BuyShow>> GetTrailerByMovie(int userId,int movieId);
        Task<IEnumerable<BuyShow>> GetTrailerByShow(int userId, int showId);
        Task<IEnumerable<Movie>> GetMoviesByDateRange(DateTime datumOd, DateTime datumDo);
        Task<IEnumerable<Movie>> GetMoviesReportAsync(long? id);
        Task<long> CountRowsInMovieAsync();
    }
}
