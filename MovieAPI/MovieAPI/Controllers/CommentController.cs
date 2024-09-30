using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController:ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentRepository _commentRepository;


        public CommentController(ILogger<CommentController> logger, ICommentRepository commentRepository)
        {
            _logger = logger;
            _commentRepository = commentRepository;
        }
        [HttpPost("Insert")]
        public async Task<ActionResult> CreateComment(Comment comment)
        {
            if (ModelState.IsValid)
            {

                var newId = await _commentRepository.CreateComment(comment);

                return Ok(newId);
            }
            else return BadRequest();
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
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteComment(int id)
        {

            var existingItem = await _commentRepository.GetCommentById(id);

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _commentRepository.DeleteComment(id);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("GetCommentById")]
        public async Task<Comment> GetById(int id)
        {
            return await _commentRepository.GetCommentById(id)
;
        }
        [HttpPut("Update")]
        public async Task<ActionResult> Put(int id, Comment editDto)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _commentRepository.GetCommentById(id)
;

                if (existingItem is null)
                {
                    return NotFound();
                }

                var numRecords = await _commentRepository.UpdateComment(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();
        }

    }
}
