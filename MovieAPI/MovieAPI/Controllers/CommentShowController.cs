using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentShowController : ControllerBase
    {
        private readonly ILogger<CommentShowController> _logger;
        private readonly ICommentShowRepository _commentShowRepository;


        public CommentShowController(ILogger<CommentShowController> logger, ICommentShowRepository commentShowRepository)
        {
            _logger = logger;
            _commentShowRepository = commentShowRepository;
        }
        [HttpPost("Insert")]
        public async Task<ActionResult> CreateComment(CommentShow comment)
        {
            if (ModelState.IsValid)
            {

                var newId = await _commentShowRepository.CreateComment(comment);

                return Ok(newId);
            }
            else return BadRequest();
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
        public async Task<ActionResult> DeleteComment(int id)
        {

            var existingItem = await _commentShowRepository.GetCommentById(id);

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _commentShowRepository.DeleteComment(id);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("GetCommentById")]
        public async Task<CommentShow> GetById(int id)
        {
            return await _commentShowRepository.GetCommentById(id)
;
        }
        [HttpPut("Update")]
        public async Task<ActionResult> Put(int id, CommentShow editDto)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _commentShowRepository.GetCommentById(id)
;

                if (existingItem is null)
                {
                    return NotFound();
                }

                var numRecords = await _commentShowRepository.UpdateComment(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();
        }

    }
}
