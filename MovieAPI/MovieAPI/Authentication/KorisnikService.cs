//using Dapper;
////using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using MovieAPI.Authentication;
//using MovieAPI.Models;
//using Npgsql;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime;
//using System.Threading.Tasks;
//using RegisterRequest = MovieAPI.Models.RegisterRequest;


//namespace MovieAPI.Authentication
//{
//    public class KorisnikService : IKorisnikService
//    {
//        private readonly ILogger<KorisnikService> _logger;
//        private IConfiguration _configuration { get; set; }
        

//        public KorisnikService(ILogger<KorisnikService> logger, IConfiguration Configuration)
//        {
//            _logger = logger;
//            _configuration = Configuration;
//        }

//        public User ProveriKorisnikAkreditivi(string username, string password)
//        {
//            _logger.LogInformation($"Проверка на логирање за корисник: [{username}]");
//            if (string.IsNullOrWhiteSpace(username))
//            {
//                return null;
//            }

//            if (string.IsNullOrWhiteSpace(password))
//            {
//                return null;
//            }

//            return Authenticate(username, password);
//        }

//        public async Task<IEnumerable<User>> ZemiKorisniciAsync()
//        {
//            using var conn = new NpgsqlConnection(_configuration["PG_Database"].ToString());

//            string sql = @"SELECT active, ""password"", username, phone, ""name"", id, salt, lastname, administrator
//                           FROM public.User
//	                     ORDER BY id";

//             return await conn.QueryAsync<User>(sql);
                
            
//        }

     
 

//        /// <summary>
//        /// Автентикација на внесените податоци за корисник и лозинка во базата
//        /// </summary>
//        /// <param name="username">Внесени податоци за корисникот</param>
//        /// <param name="password">Внесени податоци за корисникот</param>
//        /// <returns></returns>
//        private User Authenticate(string username, string password)
//        {
//            User korisnik = ZemiKorisnik(username);

//            // ако има таков корисник
//            if (korisnik is not null)
//            {
//                // Провери дали корисникот е активен
//                if (korisnik.active == true)
//                {
//                    // Земи го уникатниот salt за бараниот корисник и спреми го за хаширање
//                    byte[] salt = Convert.FromBase64String(korisnik.Salt);

//                    // Внесената лозинка при логирањето хаширај ја со додаден salt
//                    string pass_to_check = Security.HashPassword(password, salt);

//                    // Проверка на совпаѓање на лозинката во базата со ново хашираната внесена лозинка
//                    if (pass_to_check == korisnik.Password)
//                    {
//                        // Врати податоци за успешно логираниот корисник
//                        return korisnik;
//                    }
//                }

//            }
//            return null;
//        }

//        public bool ProveriPostoeckiKorisnik(string username)
//        {
//            User korisnik = ZemiKorisnik(username);

//            return korisnik is not null;
//        }

        

//        public User ZemiKorisnik(string username)
//        {
//            using NpgsqlConnection conn = new(_configuration["PG_Database"].ToString());

//                string sql = @"SELECT active, ""password"", username, phone, ""name"", id, salt, lastname, administrator
//                           FROM public.User	
//	                        where username= @username";

//                User korisnik = conn.Query<User>(sql, new { username }).FirstOrDefault();

//                return korisnik;
            
//        }

//        public void KreirajKorisnik(RegisterRequest request)
//        {

//            byte[] new_salt = Security.GenerateSalt();
//            string salt = Convert.ToBase64String(new_salt);
//            string pass = Security.HashPassword(request.Pass, new_salt);

//            using NpgsqlConnection conn = new(_configuration["PG_Database"].ToString());
//            {
    

//                string sql2 = @"INSERT INTO public.""User""(active, ""password"", username, phone, ""name"", id, salt, lastname, administrator) 
//                                    VALUES (@active, @password, @username, @phone,@name,@id,@salt,@lastname,@administrator) ";

//                var administrator=request.Administrator_bool == true ? 1 : 0;
//                var active = request.Aktiven_bool == true ? 1 : 0;

//                conn.Execute(sql2, new { request.Username, request.Salt, active, administrator });
//            }
//        }

//        public void AzurirajKorisnik(string username, UpdateUserRequest request)
//        {
//            using NpgsqlConnection conn = new(_configuration["PG_Database"].ToString());
            
              

//                string sql2 = @"update User
//                                set active = @active,
//                                    administrator = @administrator
//                                where username = @username";



//                var administrator = request.Administrator_bool == true ? 1 : 0;
//                var active = request.Aktiven_bool == true ? 1 : 0;

//                conn.Execute(sql2, new { request.Username, active, administrator });

//                if (request.Pass != null && request.Pass != "")
//                {
//                    // Генерирај уникатниот salt за корисникот
//                    byte[] new_salt = Security.GenerateSalt();
//                    // Претвори го salt-от во стринг за да се зачува во базата
//                    string salt = Convert.ToBase64String(new_salt);
//                    // Внесената лозинка хаширај ја со додаден salt
//                    string pass = Security.HashPassword(request.Pass, new_salt);

//                    string sql3 = @"UPDATE User
//	                              SET username = @username,
//		                              password = @password,
//                                      salt=@salt,                            
//                                where TRIM(USERNAME) = TRIM(@USERNAME)";

//                    conn.Execute(sql3, new { request.Username, pass, salt });
//                }
            
//        }

//        public void AzurirajProfil(string username, UpdateProfilRequest request)
//        {
//            using var conn = new NpgsqlConnection(_configuration["PG_Database"].ToString());
//            {
//                string sql = @"update User
//                                set name = @name,
//                                    lastname = @lastname
//                                where username = @username";


//                conn.Execute(sql, new { username, request.Ime, request.Prezime });

//                if (request.Pass != null && request.Pass != "")
//                {
//                    // Генерирај уникатниот salt за корисникот
//                    byte[] new_salt = Security.GenerateSalt();
//                    // Претвори го salt-от во стринг за да се зачува во базата
//                    string salt = Convert.ToBase64String(new_salt);
//                    // Внесената лозинка хаширај ја со додаден salt
//                    string pass = Security.HashPassword(request.Pass, new_salt);

//                    string sql3 = @"UPDATE User
//	                              SET username = @username,
//		                              password = @password,
//                                      salt=@salt,                            
//                                where TRIM(USERNAME) = TRIM(@USERNAME)";

//                    conn.Execute(sql3, new { username, pass, salt });
//                }
//            }
//        }

//        public int BrisiKorisnik(string username)
//        {
//            using var conn = new NpgsqlConnection(_configuration["PG_Database"].ToString());

//            String sql = @"delete from User 
//                                where TRIM(username) = TRIM(@username)";

//                return conn.Execute(sql, new { username });
            
            
//        }

        
//    }
//}
