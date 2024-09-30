using Dapper;
using MovieAPI.Models;
using Npgsql;

namespace MovieAPI.Repository
{
    public class CommentShowRepository : ICommentShowRepository
    {
        private IConfiguration _configuration { get; set; }
        public CommentShowRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public async Task<int> CreateComment(CommentShow comment)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            String sql = @"INSERT INTO public.commentshow (user_id,show_id,  rating, content ,date) 
                           VALUES (@user_id,@show_id,@rating,@content , @date) 
                           RETURNING id;";
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                comment.User_Id,
                comment.Show_Id,
                comment.Rating,
                comment.Content,
                comment.Date
            }
            );
        }
        public async Task<IEnumerable<CommentShow>> GetAllComments()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT c.show_id, c.user_id, c.date, c.rating, c.content , m.name as ShowName, u.name as UserName
                   FROM public.commentshow c
                   LEFT JOIN public.user u on u.id = c.userid
                   LEFT JOIN public.show m on m.id=c.movieid;";


            return await conn.QueryAsync<CommentShow>(sql);
        }
        public async Task<IEnumerable<CommentShow>> GetComment(int showId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT c.show_id, c.user_id, c.date, c.rating, c.content , m.name as ShowName, u.name as UserName ,c.id
                   FROM public.commentshow c
                   LEFT JOIN public.user u on u.id = c.user_id
                   LEFT JOIN public.show m on m.id=c.show_id
                   WHERE c.show_id=@showId;";


            return await conn.QueryAsync<CommentShow>(sql, new { showId });
        }

        public async Task<int> DeleteComment(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"DELETE FROM public.commentshow WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<CommentShow> GetCommentById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT id,  user_id,show_id, rating, content, date
                        FROM public.commentshow
                        WHERE id=@id;";

            var result = await conn.QueryAsync<CommentShow>(sql, new { id });
            return result.FirstOrDefault();

        }
        public async Task<int> UpdateComment(int id, CommentShow editDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"UPDATE public.commentshow
                           SET show_id=@show_id,
                               user_id=@user_id,
                               date = @date,
                               rating = @rating,
                               content = @content
                           WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new
            {
                id,
                editDto.Show_Id,
                editDto.User_Id,
                editDto.Date,
                editDto.Rating,
                editDto.Content
            });
        }

    }
}
 
