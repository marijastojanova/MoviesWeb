using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Models;
using Newtonsoft.Json;
using System.Text;

namespace MoviesWeb.Controllers
{
    public class ShowController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri _baseUrl = new Uri("https://localhost:7051/api");
        private string userId;
        public ShowController(IHttpContextAccessor context)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUrl;
            userId = context.HttpContext.Session.GetString("userId");

        }
        private async Task<User> GetCurrentUserAsync()
        {
            User user = new User();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/User/GetUserById?id=" + userId).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(data);

            }
            return user;


        }
        [HttpGet]
        public async Task<IActionResult> Index(int? genre)
        {
            List<Show> shows = new List<Show>();
            Dictionary<int, List<Show>> groupedShows = new Dictionary<int, List<Show>>();

            try
            {
                var currentUser = await GetCurrentUserAsync();
                var isAdmin = currentUser?.Administrator ?? false;
                HttpResponseMessage response;
                if (genre.HasValue && genre.Value > 0)
                {
                    response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Show/GetShowsByGenre/GetShowsByGenre?genre=" + genre).Result;
                }
                else
                {
                    response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Show/GetAll/GetAllShows").Result;
                   
                } 
                if (response.IsSuccessStatusCode)
                {
                        string data = await response.Content.ReadAsStringAsync();
                        shows = JsonConvert.DeserializeObject<List<Show>>(data);
                }
                else
                {
                        TempData["errorMessage"] = "Failed to retrieve movies.";
                }
                groupedShows = shows
                 .Where(m => m.Genre.HasValue)
                 .GroupBy(m => m.Genre.Value)
                 .ToDictionary(g => g.Key, g => g.ToList());

                ViewData["GroupedShows"] = groupedShows;
                ViewData["IsAdmin"] = isAdmin;
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View(shows);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Show show)
        {
            try
            {
                if (show.ImageFile != null && show.ImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await show.ImageFile.CopyToAsync(memoryStream);
                        show.Img_url = memoryStream.ToArray();
                    }
                }
                string data = JsonConvert.SerializeObject(show);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/Show/Post/InsertShow", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Show Created";
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
        public IActionResult Edit(int id)
        {
            try
            {
                Show show = new Show();
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Show/GetById/GetShowById?id=" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    show = JsonConvert.DeserializeObject<Show>(data);
                }
                return View(show);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        [HttpPost]
        public IActionResult Edit(Show show)
        {
            try
            {
                string data = JsonConvert.SerializeObject(show);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync(_httpClient.BaseAddress + "/Show/Put/Update?id=" + show.Id, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Show details updated";
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
                Show show = new Show();
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Show/GetById/GetShowById?id=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    show = JsonConvert.DeserializeObject<Show>(data);

                }
                return View(show);
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
                HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Show/Delete/Delete?key=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Show details deleted";
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
        public async Task<IActionResult> Search(string keyword)
        {
            List<Show> shows = new List<Show>();
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    TempData["errorMessage"] = "Keyword is required.";
                    return View("Index", shows);
                }
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Show/SearchShows/SearchShows?keyword=" + keyword).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    shows = JsonConvert.DeserializeObject<List<Show>>(data);
                }
                else
                {
                    TempData["errorMessage"] = "Failed to retrieve movies.";
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }

            return View("Search", shows);
        }
        [HttpGet]
        public IActionResult CreateTicketShow()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateTicketShow(BuyShow buyShow)
        {
            try
            {
                string data = JsonConvert.SerializeObject(buyShow);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/Show/CreateTicket/Status", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Show Created";
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
        [HttpGet, ActionName("DeleteTicketShowConfirmed")]
        public IActionResult DeleteTicket(int id)
        {

            try
            {
                HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Show/DeleteTicket/Status?id=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Show details deleted";
                    return RedirectToAction("Show");

                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
            return View("Show");
        }

        [HttpGet]
        public async Task<IActionResult> Buy(int showId)
        {
            List<BuyTicket> shows = new List<BuyTicket>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Show/GetShowsByTicket/Status?showId=" + showId + "&userId=" + userId).Result;

            if (response.IsSuccessStatusCode)

            {
                string data = await response.Content.ReadAsStringAsync();
                shows = JsonConvert.DeserializeObject<List<BuyTicket>>(data);

            }
            else
            {
                TempData["errorMessage"] = "Failed to retrieve movies.";
            }

            return RedirectToAction("Show", "Movie");
        }
     

    }
}
