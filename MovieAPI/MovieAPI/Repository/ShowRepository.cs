using Dapper;
using MovieAPI.Models;
using Npgsql;

namespace MovieAPI.Repository
{
    public class ShowRepository : IShowRepository
    {
        private IConfiguration _configuration { get; set; }
        public ShowRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public async Task<IEnumerable<Show>> GetAllShows()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name, m.id, m.img_url, m.genre, g.naziv, m.status
                   FROM public.show m
                   LEFT JOIN genre g on g.id = m.genre
                   ORDER BY m.id;";

            return await conn.QueryAsync<Show>(sql);

        }

        public async Task<Show> GetShowById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT amount, release_date, duration, name, id, genre,img_url
                   FROM public.show
                    WHERE id=@id;";

            var result = await conn.QueryAsync<Show>(sql, new { id });
            return result.FirstOrDefault();
        }

        public async Task<int> InsertShow(Show newDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            String sql = @"INSERT INTO public.""show"" (id, amount, release_date, duration, ""name"", img_url, genre, status)
                           VALUES ( (SELECT MAX(id) FROM show)+ 1,@amount, @release_date, @duration, @name,@img_url, @genre ,@status) 
                           RETURNING id;";
            return await conn.ExecuteScalarAsync<int>(sql, new
            { 
                newDto.Id,
                newDto.Amount,
                newDto.Release_date,
                newDto.Duration,
                newDto.Name,
                newDto.Img_url,
                newDto.Genre,
                newDto.Status
            }
            );

        }

        public async Task<int> UpdateShow(int id, Show editDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"UPDATE public.show
                           SET amount = @amount,
                               release_date = @release_date,
                               duration = @duration,
                               name = @name,
                               genre=@genre,
                               status=@status,
                               trailer_link=@trailer_link
                           WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new
            {
                id,
                editDto.Amount,
                editDto.Release_date,
                editDto.Duration,
                editDto.Name,
                editDto.Id,
                editDto.Genre,
                editDto.Status,
                editDto.Trailer_Link
            });
        }


        public async Task<int> DeleteShow(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"DELETE FROM public.show WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new { id });

        }

        public async Task<IEnumerable<Show>> GetShowsByGenre(int genre)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name, m.id, m.img_url, m.genre, g.naziv, m.status
                   FROM public.show m
                   LEFT JOIN genre g on g.id = m.genre
                   WHERE m.genre = @genre
                   ORDER BY m.id;";

            return await conn.QueryAsync<Show>(sql, new { genre });
        }

        public async Task<int> CreateTicket(BuyTicket buyShow)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            String sql = @"INSERT INTO public.buyticket (user_id,show_id) 
                           VALUES (@user_id,@show_id) 
                           RETURNING id;";
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                buyShow.User_Id,
                buyShow.Show_Id

            }
            );
        }

        public async Task<int> DeleteTicket(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"DELETE FROM public.buyticket WHERE id = @id;";
            return await conn.ExecuteAsync(sql, new { id });
        }

        public async Task<IEnumerable<BuyTicket>> GetAllByTicket(int userId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name as ShowName, t.id, m.img_url, m.genre , u.name,t.show_id
                   FROM public.buyticket t
                   LEFT JOIN public.user u on u.id = t.user_id
                   LEFT JOIN public.show m on m.id=t.show_id
                  
                   where t.user_id=@userId
                    ORDER BY t.id;";


            return await conn.QueryAsync<BuyTicket>(sql, new { userId });
        }
        public async Task<Show> GetTicketShowById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT user_id,show_id,id
                   FROM public.buyticket
                    WHERE id=@id;";

            var result = await conn.QueryAsync<Show>(sql, new { id });
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<BuyTicket>> DeleteAll(int userId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"DELETE FROM buyticket b WHERE b.user_id=@userId;";

            return await conn.QueryAsync<BuyTicket>(sql, new { userId });
        }
        public async Task<IEnumerable<Show>> GetShowsByDateRange(DateTime datumOd, DateTime datumDo)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using var conn = new NpgsqlConnection(connectionString);

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name, m.id, m.img_url, m.genre, g.naziv
                   FROM public.show m
                   LEFT JOIN genre g on g.id = m.genre
                   WHERE m.release_date BETWEEN @datumOd AND @datumDo
                   ORDER BY m.id;";

            return await conn.QueryAsync<Show>(sql, new { datumOd, datumDo });
        }
        public async Task<IEnumerable<Show>> GetShowsReportAsync(long? id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT amount, release_date, duration, name, id, genre
                   FROM public.show
                    WHERE id=@id";

            var shows = await conn.QueryAsync<Show>(sql, new { id });

            // Convert Movie to MovieReports
            var reportData = shows.Select(m => new Show
            {
                Id = m.Id,
                Amount = m.Amount,
                Release_date = m.Release_date,
                Duration = m.Duration,
                Name = m.Name,
                Genre = m.Genre
            });

            return reportData;

        }
        public async Task<long> CountRowsInShowAsync()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = "SELECT count_rows_in_show();";

            var result = await conn.ExecuteScalarAsync<long>(sql);
            return result;
        }

    }
}
