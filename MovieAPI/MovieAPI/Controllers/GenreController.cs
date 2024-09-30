using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GenreController:ControllerBase
    {
        private readonly ILogger<GenreController> _logger;
        private readonly IGenreRepository _genreRepository;


        public GenreController(ILogger<GenreController> logger, IGenreRepository genreRepository)
        {
            _logger = logger;
            _genreRepository = genreRepository;
        }

        [HttpGet("GetAllGenres")]
        public async Task<ActionResult<Genre>> GetAll()
        {
            return Ok(await _genreRepository.GetAllGenres());
        }


        [HttpGet("GetGenreById")]
        public async Task<Genre> GetById(int id)
        {
            return await _genreRepository.GetGenreById(id)
;
        }
    }
}
