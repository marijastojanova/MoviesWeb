using MovieAPI.Models;

namespace MovieAPI.Repository
{
    public interface ICommentShowRepository
    {
        Task<int> CreateComment(CommentShow comment);
        Task<IEnumerable<CommentShow>> GetAllComments();
        Task<IEnumerable<CommentShow>> GetComment(int showId);
        Task<int> DeleteComment(int id);
        Task<CommentShow> GetCommentById(int id);
        Task<int> UpdateComment(int id, CommentShow editDto);
    }
}
