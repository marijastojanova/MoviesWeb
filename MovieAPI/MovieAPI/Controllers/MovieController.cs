using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieRepository _movieRepository;


        public MovieController(ILogger<MovieController> logger, IMovieRepository movieRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
        }

        [HttpGet("GetAllMovies")]
        public async Task<ActionResult<Movie>> GetAll()
        {
            return Ok(await _movieRepository.GetAllMovies());

        }



        [HttpGet("GetMovieById")]
        public async Task<Movie> GetById(int id)
        {
            return await _movieRepository.GetMovieById(id)
;
        }

        [HttpPost("Insert")]
        public async Task<ActionResult> Post(/*[FromForm]*/ Movie newDto /*[FromForm] IFormFile posterFile*/)
        {
            if (ModelState.IsValid)
            {
                //if (posterFile != null && posterFile.Length > 0)
                //{
                //    using (var memoryStream = new MemoryStream())
                //    {
                //        await posterFile.CopyToAsync(memoryStream);
                //        newDto.ImageUrl = memoryStream.ToArray(); 
                //    }
                //}
                var newId = await _movieRepository.InsertMovie(newDto);

                return Ok(newId);
            }
            else return BadRequest();
        }


        [HttpPut("Update")]
        public async Task<ActionResult> Put(int id, Movie editDto)
        {
            if (ModelState.IsValid)
            {

                var existingItem = await _movieRepository.GetMovieById(id)
;

                if (existingItem is null)
                {
                    return NotFound();
                }

                var numRecords = await _movieRepository.UpdateMovie(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();
        }


        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete(int key)
        {

            var existingItem = await _movieRepository.GetMovieById(key)
;

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _movieRepository.DeleteMovie(key)
;

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetMoviesByGenre")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByGenre(int genre)
        {

            return Ok(await _movieRepository.GetMoviesByGenre(genre));
        }
        [HttpGet("SearchMovies")]
        public async Task<IActionResult> SearchMovies([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required.");
            }

            var allMovies = await _movieRepository.GetAllMovies();
            var filteredMovies = allMovies
                .Where(movie => movie.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredMovies.Any())
            {
                return NotFound("No movies found.");
            }

            return Ok(filteredMovies);
        }
        [HttpDelete("Status")]
        public async Task<ActionResult> DeleteTicket(int id)
        {

            var existingItem = await _movieRepository.GetTicketById(id);
            
            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _movieRepository.DeleteTicket(id);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("Status")]
        public async Task<ActionResult> CreateTicket(BuyTicket buyTicket)
        {
            if (ModelState.IsValid)
            {
                var newId = await _movieRepository.CreateTicket(buyTicket);

                return Ok(newId);
            }
            else return BadRequest();
        }
        [HttpGet("Status")]
        public async Task<ActionResult> GetMoviesByTicket(int movieId,int userId)
        {
           
            BuyTicket newDto = new BuyTicket();
            newDto.Movie_Id = movieId;
            newDto.User_Id = Convert.ToInt32(userId);
            var tickets = await _movieRepository.GetAllByTicket(Convert.ToInt32(userId));
            if (tickets.Any(t => t.Movie_Id == movieId))
            {
                return Ok(tickets);
            }
            else
            {
                var result = await _movieRepository.CreateTicket(newDto);
                var movies = await _movieRepository.GetAllByTicket(Convert.ToInt32(userId));
                return Ok(movies);
            }
        }
        [HttpGet("GetAllTickets")]
        public async Task<ActionResult<BuyTicket>> GetAllTickets(int userId)
        {
            
            return Ok(await _movieRepository.GetAllByTicket(Convert.ToInt32(userId)));
        }

        [HttpDelete("DeleteAllBuyTicket")]
        public async Task<ActionResult> DeleteAll(int userId)
        {
            var success = await _movieRepository.InsertAndDeleteAll(userId);
            if (success)
            {
                return Ok("Tickets inserted into buyshow and old tickets deleted successfully.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to insert and delete tickets.");
            }
        }
        [HttpGet("GetAllShow")]
        public async Task<ActionResult<BuyShow>> GetAllShow(int userId)
        {

            return Ok(await _movieRepository.GetAllByShow(Convert.ToInt32(userId)));
        }
        [HttpGet("GetTrailer")]
        public async Task<ActionResult<BuyShow>> GetTrailer(int userId,int movieId,int showId)
        {
            BuyShow trailer;

            if (!movieId.Equals(0))
            {
                trailer = (await _movieRepository.GetTrailerByMovie(Convert.ToInt32(userId), movieId)).FirstOrDefault();
            }
            else
            {
                trailer = (await _movieRepository.GetTrailerByShow(Convert.ToInt32(userId), showId)).FirstOrDefault();
            }
            return trailer;

        }
    }

}