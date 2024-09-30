using Microsoft.AspNetCore.Mvc;
using MoviesWeb.Models;
using Newtonsoft.Json;
using System.Text;

namespace MoviesWeb.Controllers
{
    public class TicketController : Controller
    {
        private readonly HttpClient _httpClient;
        Uri _baseUrl = new Uri("https://localhost:7051/api");

        public TicketController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUrl;

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Ticket> tickets = new List<Ticket>();
         
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Ticket/GetAllTickets").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                tickets = JsonConvert.DeserializeObject<List<Ticket>>(data);
            }
            else
            {
                TempData["errorMessage"] = "Failed to retrieve movies.";
            }
   

            return View(tickets);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Ticket ticket)
        {
            try
            {
                string data = JsonConvert.SerializeObject(ticket);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "/Ticket/Insert", content).Result;
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
        public IActionResult Delete(int id)
        {

            try
            {
                Ticket ticket = new Ticket();
                HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "/Ticket/GetTicketById?id=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    ticket = JsonConvert.DeserializeObject<Ticket>(data);

                }
                return View(ticket);
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
                HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "/Ticket/Delete?key=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Movie details deleted";
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

    }
}
