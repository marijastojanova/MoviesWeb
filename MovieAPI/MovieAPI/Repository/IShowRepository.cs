using MovieAPI.Models;

namespace MovieAPI.Repository
{
    public interface IShowRepository
    {
        Task<IEnumerable<Show>> GetAllShows();
        Task<Show> GetShowById(int id);
        Task<int> InsertShow(Show newDto);
        Task<int> UpdateShow(int id, Show editDto);
        Task<int> DeleteShow(int id);
        Task<IEnumerable<Show>> GetShowsByGenre(int genre);
        Task<int> CreateTicket(BuyTicket buyShow);
        Task<int> DeleteTicket(int id);
        Task<IEnumerable<BuyTicket>> GetAllByTicket(int userId);
        Task<Show> GetTicketShowById(int id);
        Task<IEnumerable<BuyTicket>> DeleteAll(int userId);
        Task<IEnumerable<Show>> GetShowsByDateRange(DateTime datumOd, DateTime datumDo);
        Task<IEnumerable<Show>> GetShowsReportAsync(long? id);
        Task<long> CountRowsInShowAsync();
    }
}
