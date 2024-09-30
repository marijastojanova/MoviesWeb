using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserWebController : ControllerBase
    {
        private readonly ILogger<UserWebController> _logger;
        private readonly IUserRepository _userRepository;


        public UserWebController(ILogger<UserWebController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<Show>> GetAll()
        {
            return Ok(await _userRepository.GetAllUsers());

        }
        [HttpPut("Update")]
        public async Task<ActionResult> Put([FromForm] int key, [FromForm] string values)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _userRepository.GetUserById(key);

                if (existingItem is null)
                {
                    return NotFound();
                }
                JsonConvert.PopulateObject(values, existingItem);
                var numRecords = await _userRepository.UpdateUser(key, existingItem);

                return Ok(numRecords);
            }
            else return BadRequest();
        }

        [HttpPost("Insert")]
        public async Task<ActionResult> Post([FromForm] string values)
        {
            if (ModelState.IsValid)
            {
                User newModul = new User();
                JsonConvert.PopulateObject(values, newModul);

                var newId = await _userRepository.InsertUser(newModul);

                return Ok(newId);
            }
            else return BadRequest();
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete([FromForm] int key)
        {
            var existingItem = await _userRepository.GetUserById(key);

            if (existingItem is null)
            {
                return NotFound();
            }

            try
            {
                var numRecords = await _userRepository.DeleteUser(key);

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest loginRequest)
        {
            
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Email and password are required.");
            }

            
            var user = await _userRepository.Login(loginRequest.Email, loginRequest.Password,loginRequest.Id);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(user);
        }
        [HttpGet("CountUsers")]
        public async Task<ActionResult<long>> CountUsers()
        {
            try
            {
                var count = await _userRepository.CountRowsInUserAsync();

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while counting rows in movie.");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
