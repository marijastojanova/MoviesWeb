using MovieAPI.Models;

namespace MovieAPI.Repository
{
    public interface IUserRepository
    {
        Task<User> Login(string username, string password,int id);
        bool VerifyPassword(string inputPassword, string storedPasswordHash, string salt);
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<int> UpdateUser(int id, User editDto);
        Task<int> DeleteUser(int id);
        Task<int> InsertUser(User newDto);
        Task<User> GetUserByEmail(string email);
        Task<int> UpdatePassword(string email, User editDto);
        Task<long> CountRowsInUserAsync();
    }
}
