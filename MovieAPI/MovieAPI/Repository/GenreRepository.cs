using Dapper;
using Microsoft.Extensions.Configuration;
using MovieAPI.Models;
using Npgsql;

namespace MovieAPI.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private IConfiguration _configuration { get; set; }
        public GenreRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT id, naziv
                   FROM public.genre
                   ORDER BY id;";

            return await conn.QueryAsync<Genre>(sql);
        }

        public async Task<Genre> GetGenreById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT id, naizv
                   FROM public.genre
                    WHERE id=@id;";

            var result = await conn.QueryAsync<Genre>(sql, new { id });
            return result.FirstOrDefault();
        }
    }

}
