using MovieAPI.Models;
using Dapper;
using Npgsql;

namespace MovieAPI.Repository
{
    public class MovieRepository : IMovieRepository 
    {
        private IConfiguration _configuration { get; set; }
        public MovieRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name, m.id, m.imageurl, m.genre, g.naziv, m.status, m.description
                   FROM public.movie m
                   LEFT JOIN genre g on g.id = m.genre
                   ORDER BY m.id ;";

            var movies= await conn.QueryAsync<Movie>(sql);
            return movies;

        }

        public async Task<Movie> GetMovieById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT amount, release_date, duration, name, id, genre,imageurl
                   FROM public.movie
                    WHERE id=@id;";

            var result = await conn.QueryAsync<Movie>(sql, new { id });
            return result.FirstOrDefault();
        }
        public async Task<IEnumerable<Movie>> GetMoviesReportAsync(long? id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            using NpgsqlConnection conn = new(connectionString);
            
                string sql = @"SELECT amount, release_date, duration, name, id, genre,imageurl
                   FROM public.movie
                    WHERE id=@id";

                var movies = await conn.QueryAsync<Movie>(sql, new { id });

                // Convert Movie to MovieReports
                var reportData = movies.Select(m => new Movie
                {
                    Id = m.Id,
                    Amount = m.Amount,
                    Release_date = m.Release_date,
                    Duration = m.Duration,
                    Name = m.Name,
                    Genre = m.Genre,
                    ImageUrl = m.ImageUrl
                });

                return reportData;
            
        }

        public async Task<int> InsertMovie(Movie newDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            String sql = @" INSERT INTO public.movie (amount, release_date, duration, ""name"", id, genre, status,imageurl)
                           VALUES (@amount, @release_date, @duration, @name, (SELECT MAX(id) FROM movie)+ 1, @genre, @status,@imageurl)
                           RETURNING id;";
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                newDto.Amount,
                newDto.Release_date,
                newDto.Duration,
                newDto.Name,
                newDto.Id,
                newDto.Genre,
                newDto.Status,
                newDto.ImageUrl
            }
            );

        }

        public async Task<int> UpdateMovie(int id, Movie editDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"UPDATE public.Movie
                           SET amount = @amount,
                               release_date = @release_date,
                               duration = @duration,
                               name = @name,
                               genre=@genre,
                               status=@status
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
                editDto.Status
            });
        }


        public async Task<int> DeleteMovie(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"DELETE FROM public.movie WHERE id = @id;";
                return await conn.ExecuteAsync(sql, new { id });
            
        }
        public async Task<IEnumerable<Movie>> GetMoviesByGenre(int genre)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name, m.id, m.imageurl, m.genre, g.naziv
                   FROM public.movie m
                   LEFT JOIN genre g on g.id = m.genre
                   WHERE m.genre = @genre
                   ORDER BY m.id;";

            return await conn.QueryAsync<Movie>(sql, new { genre });
        }

        public async Task<int> CreateTicket(BuyTicket buyTicket)
        
        {

            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            String sql = @"INSERT INTO public.buyticket (user_id,movie_id) 
                           VALUES (@user_id,@movie_id) 
                           RETURNING id;";
            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                buyTicket.User_Id,
                buyTicket.Movie_Id

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

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name as MovieName, t.id, m.imageurl as ImageUrlMovie, m.genre , u.name,t.movie_id, s.amount, s.release_date, s.duration, s.name as ShowName,s.img_url as ImageUrlShow
                   FROM public.buyticket t
                   LEFT JOIN public.user u on u.id = t.user_id
                   LEFT JOIN public.movie m on m.id=t.movie_id
                   LEFT JOIN public.show s on s.id=t.show_id
                   where t.user_id=@userId
                    ORDER BY t.id;";



            return await conn.QueryAsync<BuyTicket>(sql,new { userId});
        }
        public async Task<Movie> GetTicketById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT user_id,movie_id,id,show_id
                   FROM public.buyticket
                    WHERE id=@id;";

            var result = await conn.QueryAsync<Movie>(sql, new { id });
            return result.FirstOrDefault();
        }

        public async Task<bool> InsertAndDeleteAll(int userId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {

                string selectSql = @"SELECT user_id, movie_id, show_id FROM public.buyticket WHERE user_id = @userId;";
                var tickets = await conn.QueryAsync<BuyTicket>(selectSql, new { userId }, transaction);


                foreach (var ticket in tickets)
                {

                    string insertSql = @"INSERT INTO public.buyshow (user_id, movie_id, show_id) VALUES (@user_id, @movie_id, @show_id);";
                    await conn.ExecuteAsync(insertSql, new
                    {
                        user_id = ticket.User_Id,
                        movie_id = ticket.Movie_Id,
                        show_id = ticket.Show_Id
                    }, transaction);
                }

                string deleteSql = @"DELETE FROM public.buyticket WHERE user_id = @userId;";
                await conn.ExecuteAsync(deleteSql, new { userId }, transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<BuyShow>> GetAllByShow(int userId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT  m.amount, m.release_date, m.duration, m.name as MovieName, t.id, m.imageurl as ImageUrlMovie, m.genre , u.name,t.movie_id, s.amount, s.release_date, s.duration, s.name as ShowName,m.trailer_link as TrailerLinkMovie , s.trailer_link as TrailerLinkShow,t.show_id,s.img_url as ImageUrlShow
                   FROM public.buyshow t
                   LEFT JOIN public.user u on u.id = t.user_id
                   LEFT JOIN public.movie m on m.id=t.movie_id
                   LEFT JOIN public.show s on s.id=t.show_id
                   where t.user_id=@userId
                    ORDER BY t.id;";



            return await conn.QueryAsync<BuyShow>(sql, new { userId });
        }
        public async Task<IEnumerable<BuyShow>> GetTrailerByMovie(int userId, int movieId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT  m.trailer_link as TrailerLinkMovie ,m.name as MovieName, m.imageurl as ImageUrlMovie
                   FROM public.buyshow t
                   LEFT JOIN public.user u on u.id = t.user_id
                   LEFT JOIN public.movie m on m.id=@movieId
                   where t.user_id=@userId
                    ORDER BY t.id;";



            return await conn.QueryAsync<BuyShow>(sql, new { userId,movieId });
        }
        public async Task<IEnumerable<BuyShow>> GetTrailerByShow(int userId, int showId)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT  s.trailer_link as TrailerLinkShow ,s.name as ShowName
                   FROM public.buyshow t
                   LEFT JOIN public.user u on u.id = t.user_id
                   LEFT JOIN public.show s on s.id=@showId
                   where t.user_id=@userId
                    ORDER BY t.id;";



            return await conn.QueryAsync<BuyShow>(sql, new { userId ,showId});
        }

        public async Task<IEnumerable<Movie>> GetMoviesByDateRange(DateTime datumOd, DateTime datumDo)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using var conn = new NpgsqlConnection(connectionString);

            string sql = @"SELECT m.amount, m.release_date, m.duration, m.name, m.id, m.imageurl, m.genre, g.naziv
                   FROM public.movie m
                   LEFT JOIN genre g on g.id = m.genre
                   WHERE m.release_date BETWEEN @datumOd AND @datumDo
                   ORDER BY m.id;";

            return await conn.QueryAsync<Movie>(sql, new { datumOd, datumDo });
        }
        public async Task<long> CountRowsInMovieAsync()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = "SELECT count_rows_in_movie();";

            var result = await conn.ExecuteScalarAsync<long>(sql);
            return result;
        }

    }
}

