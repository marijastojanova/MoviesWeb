using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MoviesWeb.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoviesWeb.Controllers
{
    public class CommentController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri _baseUrl = new Uri("https://localhost:7051/api");
        private string userId;
        public CommentController(IHttpContextAccessor context)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUrl;
            userId = context.HttpContext.Session.GetString("userId");

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Comment> comments = new List<Comment>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Comment/GetAll/GetAllComments").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                comments = JsonConvert.DeserializeObject<List<Comment>>(data);
            }
            else
            {
                TempData["errorMessage"] = "Failed to retrieve comments.";
            }
            ViewData["userId"] = userId;

            return View(comments);
        }
        [HttpGet]
        public IActionResult Create(int MovieID, int UserID, string Content, DateTime Date, int Rating)
        {
            var model = new RatingViewModel
            {
                MovieID = UserID,
                UserID= UserID,
                Content = Content,
                Date = Date,
                Rating = Rating
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment(RatingViewModel comment)
        {
            try
            {
                var model = new Comment
                {
                    MovieID = comment.MovieID,
                    UserID = comment.UserID,
                    Content = comment.Content,
                    Date = comment.Date,
                    Rating = comment.Rating
                };
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/Comment/CreateComment/Insert", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Movie Created";
                    return RedirectToAction("Index","Movie");
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
        public async Task<IActionResult> Rate(int id)
        {
            List<Comment> comments = new List<Comment>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Comment/GetCommentByMovie/GetCommentsByMovie?movieId=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                comments = JsonConvert.DeserializeObject<List<Comment>>(data);

               
                var totalRatings = comments.Count;
                var count5Star = comments.Count(c => c.Rating == 5);
                var count4Star = comments.Count(c => c.Rating == 4);
                var count3Star = comments.Count(c => c.Rating == 3);
                var count2Star = comments.Count(c => c.Rating == 2);
                var count1Star = comments.Count(c => c.Rating == 1);
                var averageRating = totalRatings == 0 ? 0 : comments.Average(c => c.Rating);
                var MovieID = id;
                var UserID= Convert.ToInt32(userId);
                var model = new RatingViewModel
                {
                    AverageRating = averageRating,
                    Count5Star = count5Star,
                    Count4Star = count4Star,
                    Count3Star = count3Star,
                    Count2Star = count2Star,
                    Count1Star = count1Star,
                    TotalRatings = totalRatings,
                    Comments = comments,
                    MovieID = MovieID,
                    UserID = UserID,
                    Date = DateTime.Now
                };
                TempData["userId"] = Convert.ToInt32(userId);
                TempData["movieId"] = id;
                return View(model);
            }
            else
            {
                TempData["errorMessage"] = "Failed to retrieve comments.";
                return View(new RatingViewModel { Date = DateTime.Now });
            }
        }
        [HttpGet,ActionName("DeleteCommentConfirmed")]
        public IActionResult DeleteComment(int id)
        {

            try
            {
                HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Comment/DeleteComment/Delete?id=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Movie comment deleted";
                    var movieId = TempData["movieId"];
                    return RedirectToAction("Rate",new { id=movieId});

                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
            return View("Rate");
        }
        [HttpGet]
        public IActionResult Edit(int id,int MovieID,int UserID)
        {
            try
            {
                Comment comment = new Comment();
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Comment/GetById/GetCommentById?id=" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    comment = JsonConvert.DeserializeObject<Comment>(data);
                }
                return View(comment);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }
        [HttpPost]
        public IActionResult Edit(Comment comment, int MovieID, int UserID)
        {
            try
            {
                var model = new Comment
                {
                    MovieID = comment.MovieID,
                    UserID = comment.UserID,
                    Content = comment.Content,
                    Date = comment.Date,
                    Rating = comment.Rating
                };
                string data = JsonConvert.SerializeObject(comment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync(_httpClient.BaseAddress + "/Comment/Put/Update?id=" + comment.ID, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Movie details updated";
                    return RedirectToAction("Rate", new { id = comment.MovieID });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();

            }
            return View();
        }

    }
    
}
