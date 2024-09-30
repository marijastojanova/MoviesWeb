using MovieAPI.Models;

namespace MovieAPI.Repository
{
    public interface ITicketRepository
    {
        Task<Ticket> GetTicketById(int id);
        Task<IEnumerable<Ticket>> GetAllTickets();
        Task<int> UpdateTicket(int id, Ticket editDto);
        Task<int> DeleteTicket(int id);
        Task<int> InsertTicket(Ticket newDto);
    }
}
