using Dapper;
using Microsoft.Extensions.Options;
using MovieAPI.Models;
using Npgsql;
using Org.BouncyCastle.Bcpg;
using System.Runtime;


namespace MovieAPI.Repository
{
    public class UserRepository : IUserRepository
    {

        private IConfiguration _configuration { get; set; }
        public UserRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public async Task<User> Login(string email, string password, int id)
        {

            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT u.password, u.email, u.salt,u.id
                       FROM public.user u
                       WHERE u.email = @email;";

            var user = await conn.QueryFirstOrDefaultAsync<User>(sql, new { email });

        

            return user;
        }

        public bool VerifyPassword(string inputPassword, string storedPasswordHash, string salt)
        {

            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var saltedPassword = inputPassword + salt;
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString == storedPasswordHash;
        }
        public async Task<User> GetUserById(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = @"SELECT ""password"", username, phone, ""name"", id, salt, lastname, administrator
                           FROM public.user
                            WHERE id=@id;";
                

            var result = await conn.QueryAsync<User>(sql, new { id });
            return result.FirstOrDefault();
        }

       
        public async Task<IEnumerable<User>> GetAllUsers()
            {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);

            string sql = @"SELECT password, username, phone, name, id, lastname, email,administrator
                           FROM public.user
                           ORDER BY id";

               
             return await conn.QueryAsync<User>(sql);
                
          }
        
        public async Task<int> UpdateUser(int id, User editDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"UPDATE public.user
	                              SET 
		                              password = @password,
                                      name=@name,
                                      id=@id,
                                      lastname=@lastname,
                                      email=@email,
                                      administrator=@administrator
	                            WHERE id = @id;";

                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    id,
                    editDto.Password,
                    editDto.Name,
                    editDto.LastName,
                    editDto.Email,
                    editDto.Administrator
                });
        }
        public async Task<int> DeleteUser(int id)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"DELETE FROM public.user WHERE id=@id;";

                return await conn.ExecuteAsync(sql, new { id });
            
        }
        public async Task<int> InsertUser(User newDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            String sql = @"INSERT INTO public.""user""(""password"", ""name"", id, lastname,email, administrator) 
                                    VALUES (@password, @name,(SELECT MAX(id) FROM ""user"")+ 1,@lastname,@email, @administrator) 
                                 RETURNING id;";
                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    newDto.Password,
                    newDto.Name,
                    newDto.Id,
                    newDto.LastName,
                    newDto.Email,
                    newDto.Administrator
                }
                );
            
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = @"SELECT ""password"", username, phone, ""name"", id, salt, lastname, administrator,email
                           FROM public.user
                            WHERE email=@email;";


            var result = await conn.QueryAsync<User>(sql, new { email });
            return result.FirstOrDefault();
        }
        public async Task<int> UpdatePassword(string email, User editDto)
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);
            string sql = @"UPDATE public.user
	                              SET 
		                              password = @password
	                            WHERE email = @email;";

            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                email,
                editDto.Password
            });
        }
        public async Task<long> CountRowsInUserAsync()
        {
            var connectionString = _configuration.GetConnectionString("PG_Database");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            using NpgsqlConnection conn = new(connectionString);


            string sql = "SELECT count_rows_in_users();";

            var result = await conn.ExecuteScalarAsync<long>(sql);
            return result;
        }

    }
}
 

   

 