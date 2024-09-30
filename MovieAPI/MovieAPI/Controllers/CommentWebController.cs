using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;
using Newtonsoft.Json;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentWebController : Controller
    {
        private readonly ILogger<CommentWebController> _logger;
        private readonly ICommentRepository _commentRepository;


        public CommentWebController(ILogger<CommentWebController> logger, ICommentRepository commentRepository)
        {
            _logger = logger;
            _commentRepository = commentRepository;
        }
        [HttpGet("GetAllComments")]
        public async Task<ActionResult<Comment>> GetAll()
        {
            return Ok(await _commentRepository.GetAllComments());
        }

        [HttpGet("GetCommentsByMovie")]
        public async Task<ActionResult<Comment>> GetCommentByMovie(int movieId)
        {

            return Ok(await _commentRepository.GetComment(movieId));
        }
        [HttpGet("GetCommentById")]
        public async Task<Comment> GetById(int id)
        {
            return await _commentRepository.GetCommentById(id)
;
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteComment([FromForm] int key)
        {

            var existingItem = await _commentRepository.GetCommentById(key);

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _commentRepository.DeleteComment(key);

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
                Comment newModul = new Comment();
                JsonConvert.PopulateObject(values, newModul);

  
                var newId = await _commentRepository.CreateComment(newModul);

                return Ok(newId);
            }
            else return BadRequest();
        }
        [HttpPut("Update")]
        public async Task<ActionResult> Put([FromForm] int key, [FromForm] string values)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _commentRepository.GetCommentById(key);

                if (existingItem is null)
                {
                    return NotFound();
                }
                JsonConvert.PopulateObject(values, existingItem);
                var numRecords = await _commentRepository.UpdateComment(key, existingItem);

                return Ok(numRecords);
            }
            else return BadRequest(); 
        }
    }
}
