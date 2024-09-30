using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;
using Newtonsoft.Json;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MovieWebController : ControllerBase
    {
        private readonly ILogger<MovieWebController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IWebHostEnvironment _env;

        public MovieWebController(ILogger<MovieWebController> logger, IMovieRepository movieRepository, IWebHostEnvironment env)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _env = env;
        }

        [HttpGet("GetAllMovies")]
        public async Task<ActionResult<Movie>> GetAll()
        {
            return Ok(await _movieRepository.GetAllMovies());

        }
        [HttpPut("Update")]
        public async Task<ActionResult> Put([FromForm] int id, [FromForm] Movie editDto, IFormFile? posterFile)
        {
            if (ModelState.IsValid)
            {

                var existingItem = await _movieRepository.GetMovieById(id);
                if (posterFile != null && posterFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await posterFile.CopyToAsync(memoryStream);
                        editDto.ImageUrl = memoryStream.ToArray();
                    }
                }
                if (existingItem is null)
                {
                    return NotFound();
                }

                var numRecords = await _movieRepository.UpdateMovie(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();

        }

        //[HttpPost("Insert")]
        //public async Task<ActionResult> Post([FromForm] string values)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Movie newModul = new Movie();
        //        JsonConvert.PopulateObject(values, newModul);
        //        _logger.LogInformation($"Креирање на модул: [{newModul.Naziv}]");

        //        var newId = await _movieRepository.InsertMovie(newModul);

        //        return Ok(newId);
        //    }
        //    else return BadRequest();
        //}
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete([FromForm] int key)
        {
            var existingItem = await _movieRepository.GetMovieById(key);

            if (existingItem is null)
            {
                return NotFound();
            }

            try
            {
                var numRecords = await _movieRepository.DeleteMovie(key);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("GetMoviesByDateRange")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByDateRange(DateTime datumOd, DateTime datumDo)
        {
            if (datumOd > datumDo)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            var movies = await _movieRepository.GetMoviesByDateRange(datumOd, datumDo);

            if (movies == null || !movies.Any())
            {
                return NotFound("No movies found in the specified date range.");
            }

            return Ok(movies);
        }
        [HttpGet("Report")]
        public async Task<IActionResult> GetMoviesReport(long? id)
        {
            var results = await _movieRepository.GetMoviesReportAsync(id);
            return Ok(results);
        }
        [HttpGet("CountMovies")]
        public async Task<ActionResult<long>> CountMovies()
        {
            try
            {
                var count = await _movieRepository.CountRowsInMovieAsync();

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while counting rows in movie.");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpPost("Insert")]
        public async Task<ActionResult> Post([FromForm]Movie newDto,IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {

                if (posterFile != null && posterFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await posterFile.CopyToAsync(memoryStream);
                        newDto.ImageUrl = memoryStream.ToArray();
                    }
                }
                var newId = await _movieRepository.InsertMovie(newDto);

                return Ok(newId);
            }
            else return BadRequest();
        }

    }
}


