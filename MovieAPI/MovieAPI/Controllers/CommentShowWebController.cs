using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;
using Newtonsoft.Json;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentShowWebController : ControllerBase
    {
        private readonly ILogger<CommentShowWebController> _logger;
        private readonly ICommentShowRepository _commentShowRepository;


        public CommentShowWebController(ILogger<CommentShowWebController> logger, ICommentShowRepository commentShowRepository)
        {
            _logger = logger;
            _commentShowRepository = commentShowRepository;
        }
        [HttpGet("GetAllComments")]
        public async Task<ActionResult<CommentShow>> GetAll()
        {
            return Ok(await _commentShowRepository.GetAllComments());
        }

        [HttpGet("GetCommentsByShow")]
        public async Task<ActionResult<CommentShow>> GetCommentByShow(int showId)
        {

            return Ok(await _commentShowRepository.GetComment(showId));
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteComment([FromForm] int key)
        {

            var existingItem = await _commentShowRepository.GetCommentById(key);

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _commentShowRepository.DeleteComment(key);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("Insert")]
        public async Task<ActionResult> CreateComment([FromForm]string values)
        {
            if (ModelState.IsValid)
            {
                CommentShow newModul = new CommentShow();
                JsonConvert.PopulateObject(values, newModul);


                var newId = await _commentShowRepository.CreateComment(newModul);

                return Ok(newId);
            }
            else return BadRequest();
        }
        [HttpPut("Update")]
        public async Task<ActionResult> Put([FromForm] int key, [FromForm] string values)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _commentShowRepository.GetCommentById(key);

                if (existingItem is null)
                {
                    return NotFound();
                }
                JsonConvert.PopulateObject(values, existingItem);
                var numRecords = await _commentShowRepository.UpdateComment(key, existingItem);

                return Ok(numRecords);
            }
            else return BadRequest();
        }
    }
}
