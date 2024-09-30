using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Models;
using Newtonsoft.Json;
using System.Text;

namespace MoviesWeb.Controllers
{
    public class CommentShowController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri _baseUrl = new Uri("https://localhost:7051/api");
        private string userId;
        public CommentShowController(IHttpContextAccessor context)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUrl;
            userId = context.HttpContext.Session.GetString("userId");

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CommentShow> comments = new List<CommentShow>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/CommentShow/GetAll/GetAllComments").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                comments = JsonConvert.DeserializeObject<List<CommentShow>>(data);
            }
            else
            {
                TempData["errorMessage"] = "Failed to retrieve comments.";
            }


            return View(comments);
        }
        [HttpGet]
        public IActionResult CreateComment(int Show_Id, int User_Id, string Content, DateTime Date, int Rating)
        {
            var model = new RatingViewModelShow
            {
                Show_Id = Show_Id,
                User_Id = User_Id,
                Content = Content,
                Date = Date,
                Rating = Rating
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment(RatingViewModelShow commentShow)
        {
            try
            {
                var model = new CommentShow
                {
                    Show_Id = commentShow.Show_Id,
                    User_Id = commentShow.User_Id,
                    Content = commentShow.Content,
                    Date = commentShow.Date,
                    Rating = commentShow.Rating
                };
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/CommentShow/CreateComment/Insert", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Comment Created";
                    return RedirectToAction("Rate", new { id = commentShow.Show_Id });
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
            List<CommentShow> comments = new List<CommentShow>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/CommentShow/GetCommentByShow/GetCommentsByShow?showId=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                comments = JsonConvert.DeserializeObject<List<CommentShow>>(data);


                var totalRatings = comments.Count;
                var count5Star = comments.Count(c => c.Rating == 5);
                var count4Star = comments.Count(c => c.Rating == 4);
                var count3Star = comments.Count(c => c.Rating == 3);
                var count2Star = comments.Count(c => c.Rating == 2);
                var count1Star = comments.Count(c => c.Rating == 1);
                var averageRating = totalRatings == 0 ? 0 : comments.Average(c => c.Rating);
                var Show_Id = id;
                var User_Id = Convert.ToInt32(userId);
                var model = new RatingViewModelShow
                {
                    AverageRating = averageRating,
                    Count5Star = count5Star,
                    Count4Star = count4Star,
                    Count3Star = count3Star,
                    Count2Star = count2Star,
                    Count1Star = count1Star,
                    TotalRatings = totalRatings,
                    Comments = comments,
                    Show_Id = Show_Id,
                    User_Id = User_Id,
                    Date = DateTime.Now
                };

                TempData["userId"] = Convert.ToInt32(userId);
                TempData["showId"] = id;
                return View(model);
            }
            else
            {
                TempData["errorMessage"] = "Failed to retrieve comments.";
                return View(new RatingViewModelShow { Date = DateTime.Now });
            }
        }
        [HttpGet, ActionName("DeleteCommentConfirmed")]
        public IActionResult DeleteComment(int id)
        {

            try
            {
                HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/CommentShow/DeleteComment/Delete?id=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Show comment deleted";
                    var showId = TempData["showId"];
                    return RedirectToAction("Rate", new { id = showId });

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
        public IActionResult Edit(int id, int Show_Id, int User_Id)
        {
            try
            {
                CommentShow comment = new CommentShow();
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/CommentShow/GetById/GetCommentById?id=" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    comment = JsonConvert.DeserializeObject<CommentShow>(data);
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
        public IActionResult Edit(CommentShow comment, int Show_Id, int User_Id)
        {
            try
            {
                var model = new CommentShow
                {
                    Show_Id = comment.Show_Id,
                    User_Id = comment.User_Id,
                    Content = comment.Content,
                    Date = comment.Date,
                    Rating = comment.Rating
                };
                string data = JsonConvert.SerializeObject(comment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PutAsync(_httpClient.BaseAddress + "/CommentShow/Put/Update?id=" + comment.ID, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Movie details updated";
                    return RedirectToAction("Rate", new { id = comment.Show_Id });
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
