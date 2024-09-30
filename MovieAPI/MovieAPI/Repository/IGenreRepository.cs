using MovieAPI.Models;

namespace MovieAPI.Repository
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllGenres();
        Task<Genre> GetGenreById(int id);
    }
}
