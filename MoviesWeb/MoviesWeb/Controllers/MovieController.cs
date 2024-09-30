using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MoviesWeb.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class MovieController : Controller
{
    private readonly HttpClient _httpClient;
    Uri _baseUrl = new Uri("https://localhost:7051/api");
    private string userId;
    public MovieController(IHttpContextAccessor context)
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
    public async Task<IActionResult> Index(int? genre )
    {
        List<Movie> movies = new List<Movie>();
        Dictionary<int, List<Movie>> groupedMovies = new Dictionary<int, List<Movie>>();

        try
        {

            var currentUser = await GetCurrentUserAsync();
            var isAdmin = currentUser?.Administrator ?? false;

            HttpResponseMessage response;
            if (genre.HasValue && genre.Value > 0)
            {
                response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/Movie/GetMoviesByGenre/GetMoviesByGenre?genre={genre}");
            }
            else
            {
                response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/GetAll/GetAllMovies");
            }

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                movies = JsonConvert.DeserializeObject<List<Movie>>(data);
            }
            else
            {
                TempData["errorMessage"] = "Failed to retrieve movies.";
            }

            groupedMovies = movies
             .Where(m => m.Genre.HasValue) 
             .GroupBy(m => m.Genre.Value)  
             .ToDictionary(g => g.Key, g => g.ToList());

            ViewData["GroupedMovies"] = groupedMovies;
            ViewData["IsAdmin"] = isAdmin;

        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = ex.Message;
        }

        return View(movies);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Movie movie)
    {
        try
        {
            if (movie.ImageFile != null && movie.ImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movie.ImageFile.CopyToAsync(memoryStream);
                    movie.ImageUrl = memoryStream.ToArray();
                }
            }

            string data = JsonConvert.SerializeObject(movie);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Movie/Post/Insert", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Movie Created";
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
            Movie movie = new Movie();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/GetById/GetMovieById?id=" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                movie = JsonConvert.DeserializeObject<Movie>(data);
            }
            return View(movie);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View();
        }

    }
    [HttpPost]
    public IActionResult Edit(Movie movie)
    {
        try
        {
            string data = JsonConvert.SerializeObject(movie);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.PutAsync(_httpClient.BaseAddress + "/Movie/Put/Update?id=" + movie.Id, content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Movie details updated";
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
            Movie movie = new Movie();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/GetById/GetMovieById?id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                movie = JsonConvert.DeserializeObject<Movie>(data);

            }
            return View(movie);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View();

        }
    }
    [HttpPost, ActionName("DeleteConfirmed")]
    public IActionResult DeleteConfirmed(int id) {
        try {
            HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Movie/Delete/Delete?key=" + id).Result;
            if (response.IsSuccessStatusCode) {
                TempData["successMessage"] = "Movie details deleted";
                return RedirectToAction("Index");
            }
        }
        catch (Exception ex) {
            TempData["errorMessage"] = ex.Message;
            return View();
        }
        return View("Index");
    }



    [HttpGet]
    public async Task<IActionResult> Search(string keyword)
    {
        List<Movie> movies = new List<Movie>();
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                TempData["errorMessage"] = "Keyword is required.";
                return View("Index", movies); 
            }
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/SearchMovies/SearchMovies?keyword=" + keyword).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                movies = JsonConvert.DeserializeObject<List<Movie>>(data);
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

        return View("Search", movies); 
    }
    [HttpGet]
    public IActionResult CreateTicket()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CreateTicket(BuyTicket buyTicket)
    {
        try
        {
            string data = JsonConvert.SerializeObject(buyTicket);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/Movie/CreateTicket/Status", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Movie Created";
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
    [HttpGet, ActionName("DeleteTicketConfirmed")]
    public IActionResult DeleteTicket(int id)
    {

        try
        {   
            HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Movie/DeleteTicket/Status?id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Movie details deleted";
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
    public async Task<IActionResult> Buy(int id)
    {
        List<BuyTicket> movies = new List<BuyTicket>();
        HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/GetMoviesByTicket/Status?movieId=" + id + "&userId=" + userId).Result;

        if (response.IsSuccessStatusCode)

        {
            string data = await response.Content.ReadAsStringAsync();
            movies = JsonConvert.DeserializeObject<List<BuyTicket>>(data);
            
        }
        else
        {
            TempData["errorMessage"] = "Failed to retrieve movies.";
        }
    

        return View(movies);
    }
    [HttpGet]
    public async Task<IActionResult> Show()
    {
        List<BuyTicket> movies = new List<BuyTicket>();
        HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/GetAllTickets/GetAllTickets?userId=" + userId).Result;
            if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    movies = JsonConvert.DeserializeObject<List<BuyTicket>>(data);
                }
                else
                {
                    TempData["errorMessage"] = "Failed to retrieve movies.";
                }


        return View(movies);
    }
    [HttpPost, ActionName("DeleteAllConfirmed")]
    public IActionResult DeleteAll()
    {

        try
        {
            HttpResponseMessage response =_httpClient.DeleteAsync(_httpClient.BaseAddress + "/Movie/DeleteAll/DeleteAllBuyTicket?userId=" + userId).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Cards deleted";
                return RedirectToAction("Show");
            }
            else
            {
                TempData["errorMessage"] = "Failed to delete cards.";
            }
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "An error occurred while deleting cards.";
        }

        return View("Show");
    }
    [HttpGet]
    public IActionResult Payment()
    {
        //try
        //{
        //    List<BuyTicket> tickets = new List<BuyTicket>();
        //    HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/SelectAllById/GetAllBuyTicketByUser?userId=" +userId).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        string data = response.Content.ReadAsStringAsync().Result;
        //        tickets = JsonConvert.DeserializeObject<List<BuyTicket>>(data);

        //    }
        //    return View("Payment",tickets);
        //}
        //catch (Exception ex)
        //{
        //    TempData["errorMessage"] = ex.Message;
        //    return View();

        //}
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Trailer()
    {
        List<BuyShow> movies = new List<BuyShow>();
        HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/GetAllShow/GetAllShow?userId=" + userId).Result;
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            movies = JsonConvert.DeserializeObject<List<BuyShow>>(data);
        }
        else
        {
            TempData["errorMessage"] = "Failed to retrieve movies.";
        }


        return View(movies);
    }
    [HttpGet,ActionName("Watch")]
    public async Task<IActionResult> Watch(int movieId,int showId)
    {
        BuyShow movies = new BuyShow();
        HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Movie/GetTrailer/GetTrailer?userId=" + userId + "&movieId=" + movieId +"&showId=" + showId).Result;
        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            movies = JsonConvert.DeserializeObject<BuyShow>(data);
        }
        else
        {
            TempData["errorMessage"] = "Failed to retrieve movies.";
        }


        return View(movies);
    }
}



