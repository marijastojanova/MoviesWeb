using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;
using Newtonsoft.Json;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShowWebController : ControllerBase
    {
        private readonly ILogger<ShowWebController> _logger;
        private readonly IShowRepository _showRepository;


        public ShowWebController(ILogger<ShowWebController> logger, IShowRepository showRepository)
        {
            _logger = logger;
            _showRepository = showRepository;
        }
        [HttpGet("GetAllShows")]
        public async Task<ActionResult<Show>> GetAll()
        {
            return Ok(await _showRepository.GetAllShows());

        }
        [HttpPut("Update")]
        public async Task<ActionResult> Put([FromForm] int id, [FromForm] Show editDto, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {

                var existingItem = await _showRepository.GetShowById(id);
                if (posterFile != null && posterFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await posterFile.CopyToAsync(memoryStream);
                        editDto.Img_url = memoryStream.ToArray();
                    }
                }
                if (existingItem is null)
                {
                    return NotFound();
                }

                var numRecords = await _showRepository.UpdateShow(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();

        }

        //[HttpPost("Insert")]
        //public async Task<ActionResult> Post([FromForm] string values)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Show newModul = new Show();
        //        JsonConvert.PopulateObject(values, newModul);
        //        _logger.LogInformation($"Креирање на модул: [{newModul.Naziv}]");

        //        var newId = await _showRepository.InsertShow(newModul);

        //        return Ok(newId);
        //    }
        //    else return BadRequest();
        //}
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete([FromForm] int key)
        {
            var existingItem = await _showRepository.GetShowById(key);

            if (existingItem is null)
            {
                return NotFound();
            }

            try
            {
                var numRecords = await _showRepository.DeleteShow(key);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetShowsByDateRange")]
        public async Task<ActionResult<IEnumerable<Show>>> GetShowsByDateRange(DateTime datumOd, DateTime datumDo)
        {
            if (datumOd > datumDo)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            var movies = await _showRepository.GetShowsByDateRange(datumOd, datumDo);

            if (movies == null || !movies.Any())
            {
                return NotFound("No movies found in the specified date range.");
            }

            return Ok(movies);
        }
        [HttpGet("Report")]
        public async Task<IActionResult> GetShowsReport(long? id)
        {
            var results = await _showRepository.GetShowsReportAsync(id);
            return Ok(results);
        }
        [HttpGet("CountShows")]
        public async Task<ActionResult<long>> CountShows()
        {
            try
            {
                var count = await _showRepository.CountRowsInShowAsync();

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while counting rows in movie.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("Insert")]
        public async Task<ActionResult> Post([FromForm] Show newDto, IFormFile posterFile)
        {
            if (ModelState.IsValid)
            {

                if (posterFile != null && posterFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await posterFile.CopyToAsync(memoryStream);
                        newDto.Img_url = memoryStream.ToArray();
                    }
                }
                var newId = await _showRepository.InsertShow(newDto);

                return Ok(newId);
            }
            else return BadRequest();
        }

    }

}
