using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Models;
using Newtonsoft.Json;

namespace MoviesWeb.Controllers
{
    public class GenreController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri _baseUrl = new Uri("https://localhost:7051/api");

        public GenreController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUrl;

        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Genre> genres = new List<Genre>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Genre/GetAll/GetAllGenres").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                genres = JsonConvert.DeserializeObject<List<Genre>>(data);
            }
            return View(genres);
        }
    }
}
