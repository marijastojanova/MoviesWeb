using Microsoft.Extensions.Configuration;
using MovieAPI.Models;
using Npgsql;
using Dapper;

namespace MovieAPI.Repository
{
    public class CommentRepository:ICommentRepository
    {
        private IConfiguration _configuration { get; set; }
        public CommentRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public async Task<int> CreateComment(Comment comment)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            String sql = @"INSERT INTO public.comment (movieid, userid, date, rating, content) 
                           VALUES (@movieid,@userid,@date,@rating,@content) 
                           RETURNING id;";
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                comment.MovieID,
                comment.UserID,
                comment.Date,
                comment.Rating,
                comment.Content
            }
            );
        }
        public async Task<IEnumerable<Comment>> GetAllComments()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT c.movieid, c.userid, c.date, c.rating, c.content , m.name as MovieName, u.name as UserName
                   FROM public.comment c
                   LEFT JOIN public.user u on u.id = c.userid
                   LEFT JOIN public.movie m on m.id=c.movieid;";


            return await conn.QueryAsync<Comment>(sql);
        }
        public async Task<IEnumerable<Comment>> GetComment(int movieId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT c.movieid, c.userid, c.date, c.rating, c.content , m.name as MovieName, u.name as UserName ,c.id
                   FROM public.comment c
                   LEFT JOIN public.user u on u.id = c.userid
                   LEFT JOIN public.movie m on m.id=c.movieid
                   WHERE c.movieid=@movieId;";


            return await conn.QueryAsync<Comment>(sql, new { movieId });
        }

        public async Task<int> DeleteComment(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"DELETE FROM public.comment WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<Comment> GetCommentById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT id, movieid, userid, date, rating, content
                        FROM public.comment
                        WHERE id=@id;";

            var result = await conn.QueryAsync<Comment>(sql, new { id });
            return result.FirstOrDefault();

        }
        public async Task<int> UpdateComment(int id, Comment editDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"UPDATE public.comment
                           SET movieid=@movieid,
                               userid=@userid,
                               date = @date,
                               rating = @rating,
                               content = @content
                           WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new
            {
                id,
                editDto.MovieID,
                editDto.UserID,
                editDto.Date,
                editDto.Rating,
                editDto.Content
            });
        }
    }
}
