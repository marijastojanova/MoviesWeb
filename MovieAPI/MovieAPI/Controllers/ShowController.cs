using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private readonly ILogger<ShowController> _logger;
        private readonly IShowRepository _showRepository;


        public ShowController(ILogger<ShowController> logger, IShowRepository showRepository)
        {
            _logger = logger;
            _showRepository = showRepository;
        }

        [HttpGet("GetAllShows")]
        public async Task<ActionResult<Show>> GetAll()
        {
            return Ok(await _showRepository.GetAllShows());
        }


        [HttpGet("GetShowById")]
        public async Task<Show> GetById(int id)
        {
            return await _showRepository.GetShowById(id)
;
        }

        [HttpPost("InsertShow")]
        public async Task<ActionResult> Post(Show newDto)
        {
            if (ModelState.IsValid)
            {

                var newId = await _showRepository.InsertShow(newDto);

                return Ok(newId);
            }
            else return BadRequest();
        }


        [HttpPut("Update")]
        public async Task<ActionResult> Put(int id, Show editDto)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _showRepository.GetShowById(id)
;

                if (existingItem is null)
                {
                    return NotFound();
                }

                var numRecords = await _showRepository.UpdateShow(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();
        }


        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete(int key)
        {

            var existingItem = await _showRepository.GetShowById(key)
;

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _showRepository.DeleteShow(key)
;

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("GetShowsByGenre")]
        public async Task<ActionResult<IEnumerable<Show>>> GetShowsByGenre(int genre)
        {

            return Ok(await _showRepository.GetShowsByGenre(genre));
        }
        [HttpGet("SearchShows")]
        public async Task<IActionResult> SearchShows([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            var allShows = await _showRepository.GetAllShows();
            var filteredShows = allShows
                .Where(show => show.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredShows.Any())
            {
                return NotFound("No movies found.");
            }

            return Ok(filteredShows);
        }
        [HttpDelete("Status")]
        public async Task<ActionResult> DeleteShow(int id)
        {

            var existingItem = await _showRepository.GetTicketShowById(id);

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _showRepository.DeleteTicket(id);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Status")]
        public async Task<ActionResult> CreateTicket(BuyTicket buyShow)
        {
            if (ModelState.IsValid)
            {
                var newId = await _showRepository.CreateTicket(buyShow);

                return Ok(newId);
            }
            else return BadRequest();
        }
        [HttpGet("Status")]
        public async Task<ActionResult> GetShowsByTicket(int showId, int userId)
        {

            BuyTicket newDto = new BuyTicket();
            newDto.Show_Id = showId;
            newDto.User_Id = Convert.ToInt32(userId);
            var tickets = await _showRepository.GetAllByTicket(Convert.ToInt32(userId));
            if (tickets.Any(t => t.Show_Id == showId))
            {
                return Ok(tickets);
            }
            else
            {
                var result = await _showRepository.CreateTicket(newDto);
                var movies = await _showRepository.GetAllByTicket(Convert.ToInt32(userId));
                return Ok(movies);
            }
        }
        [HttpGet("GetAllTicketsShow")]
        public async Task<ActionResult<BuyTicket>> GetAllTickets(int userId)
        {

            return Ok(await _showRepository.GetAllByTicket(Convert.ToInt32(userId)));
        }

        [HttpDelete("DeleteAllBuyTicketShow")]
        public async Task<ActionResult> DeleteAll(int userId)
        {
            return Ok(await _showRepository.DeleteAll(userId));
        }
       
    }
}
