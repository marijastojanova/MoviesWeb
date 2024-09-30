using MovieAPI.Models;
using Npgsql;
using Dapper;

namespace MovieAPI.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private IConfiguration _configuration { get; set; }
        public TicketRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
        public async Task<int> DeleteTicket(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"DELETE FROM public.ticket WHERE id=@id;";

            return await conn.ExecuteAsync(sql, new { id });
            
        }

        
        public async Task<IEnumerable<Ticket>> GetAllTickets()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = @"SELECT amount, watch_movie, user_id, movie_id, id
                          FROM public.ticket
                          ORDER BY id;";

    
            return await conn.QueryAsync<Ticket>(sql);
           
        }

        public async Task<Ticket> GetTicketById(int id)
        {

            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = @"SELECT amount, watch_movie, user_id, movie_id, id
                          FROM public.ticket
                          WHERE id=@id;";

            var result = await conn.QueryAsync<Ticket>(sql, new { id });
            return result.FirstOrDefault();
        }

        public async Task<int> InsertTicket(Ticket newDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            String sql = @"INSERT INTO public.ticket(amount, watch_movie, user_id, movie_id, id)
                                    VALUES (@amount, @watch_movie, @user_id,@movie_id,@id) 
                                 RETURNING id;";
                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    newDto.Amount,
                    newDto.watch_movie,
                    newDto.User_id,
                    newDto.Movie_id,
                    newDto.Id
                }
                );
            
        }

        public async Task<int> UpdateTicket(int id, Ticket editDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = @"UPDATE public.ticket
	                              SET amount= @amount,
		                              watch_movie = @watch_movie,
		                              user_id = @user_id,
		                              movie_id= @movie_id,
                                      id=@id
	                            WHERE id = @id;";

                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    id,
                    editDto.Amount,
                    editDto.watch_movie,
                    editDto.User_id,
                    editDto.Movie_id
                });
            
        }
    }
}
