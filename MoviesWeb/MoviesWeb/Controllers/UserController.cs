using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace MoviesWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri _baseUrl = new Uri("https://localhost:7051/api");
        private string userId;
        public UserController(IHttpContextAccessor context)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUrl;
            userId = context.HttpContext.Session.GetString("userId");
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            try
            {
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/User/Insert", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User Created";
                    return RedirectToAction("Login");
                }
            }

            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                User user = new User();
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/User/Update?id=" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(data);
                }
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        [HttpPost]
        public IActionResult Edit(User user)
        {
            try
            {
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync(_httpClient.BaseAddress + "/User/Update?id=" + user.Id, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User updated";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {

            try
            {
                User user = new User();
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/User/Delete?id=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<User>(data);

                }
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/User/Delete?key=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User deleted";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View("Index");
        }
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {

            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/User/FindUserByEmail", content);

                if (response.IsSuccessStatusCode)
                {
                    
                    string result = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(result);

                    if (user == null)
                    {
                        TempData["errorMessage"] = "User not found.";
                        return View(model);
                    }

                    HttpContext.Session.SetString("userId", user.Id.ToString());

                    TempData["successMessage"] = "Email verified. Please change your password.";
                    return RedirectToAction("Confirmation",user);
                }
                else
                {
                    TempData["errorMessage"] = "Invalid email or an error occurred.";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"An error occurred: {ex.Message}";
            }

   
            return View(model);
        }
        [HttpGet]
        public IActionResult Confirmation(User user)
        {
          
            return View(user);
        }

     
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["errorMessage"] = "Email is required.";
                return View();
            }

            try
            {
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/User/UpdatePassword?email=" + email).Result;

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(data);


                    var model = new ChangePasswordViewModel
                    {
                        Email = user.Email
                    };

                    return View(model);
                }
                else
                {
                    TempData["errorMessage"] = "User not found or an error occurred.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"An error occurred: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {

            try
            {

                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    TempData["errorMessage"] = "Passwords do not match.";
                    return View(model);
                }

                var userUpdate = new User
                {
                    Email = model.Email,
                    Password = model.NewPassword,
                    ConfirmPassword = model.ConfirmNewPassword

                };

                string data = JsonConvert.SerializeObject(userUpdate);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PutAsync(_httpClient.BaseAddress + "/User/UpdatePassword?email=" + userUpdate.Email, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Password updated successfully.";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["errorMessage"] = "Failed to update password.";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"An error occurred: {ex.Message}";
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            var savedEmail = Request.Cookies["MoviesAPIAuthCookieEmail"];
            var savedPassword = Request.Cookies["MoviesAPIAuthCookiePassword"];

   
            var model = new LoginViewModel
            {
                Email = savedEmail,
                Password=savedPassword
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
              
                try
                {
            
                string data = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/User/Login", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<User>(result);
                        var claims = new List<Claim>
                        {
                              new Claim(ClaimTypes.Name, user.Email,user.Password)
              
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    if (model.RememberMe)
                    {
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30), 
                            HttpOnly = true,
                            Secure = true, 
                            SameSite = SameSiteMode.Strict
                        };

                        Response.Cookies.Append("MoviesAPIAuthCookieEmail", model.Email, cookieOptions);
                        Response.Cookies.Append("MoviesAPIAuthCookiePassword", model.Password, cookieOptions);
                    }


                    HttpContext.Session.SetString("userId", user.Id.ToString());

                    TempData["successMessage"] = "Login successful";
                        return RedirectToAction("Index", "Movie");
                    }
                    else
                    {

                        TempData["errorMessage"] = "Invalid username or password.";
                    }
                }
                catch (Exception ex)
                {

                    TempData["errorMessage"] = $"An error occurred: {ex.Message}";
                }
            
            
            return View(model);
        }

        [HttpGet,ActionName("LogoutUser")]
        public async Task<IActionResult> Logout()
        {

            HttpContext.Session.Clear();

            return RedirectToAction("Login", new { ShowLogout = true });
        }

    }
}

