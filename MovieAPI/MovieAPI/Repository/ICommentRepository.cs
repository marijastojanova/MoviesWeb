using MovieAPI.Models;

namespace MovieAPI.Repository
{
    public interface ICommentRepository
    {
        Task<int> CreateComment(Comment comment);
        Task<IEnumerable<Comment>> GetAllComments();
        Task<IEnumerable<Comment>> GetComment(int movieId);
        Task<int> DeleteComment(int id);
        Task<Comment> GetCommentById(int id);
        Task<int> UpdateComment(int id, Comment editDto);
    }
}
