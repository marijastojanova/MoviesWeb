using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;


        public UserController(ILogger<UserController> logger, IUserRepository userRepository)
        {
            

            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<Ticket>> GetAll()
        {
            return Ok(await _userRepository.GetAllUsers());
        }

        
        [HttpGet("GetUserById")]
        public async Task<User> GetById(int id)
        {
            return await _userRepository.GetUserById(id)
;
        }



  
        [HttpPost("Insert")]
        public async Task<ActionResult> Post( User newDto)
        {
            if (ModelState.IsValid)
            {
               
                var newId = await _userRepository.InsertUser(newDto);

                return Ok(newId);
            }
            else return BadRequest();
        }

        
        [HttpPut("Update")]
        public async Task<ActionResult> Put(int id, User editDto)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _userRepository.GetUserById(id)
;

                if (existingItem is null)
                {
                    return NotFound();
                }

                editDto.Id = existingItem.Id;
                var numRecords = await _userRepository.UpdateUser(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();
        }


        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete( int key)
        {

            var existingItem = await _userRepository.GetUserById(key)
;

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _userRepository.DeleteUser(key)
;

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _userRepository.Login(loginRequest.Email, loginRequest.Password,loginRequest.Id);

         

            if (user == null)
            {
                return BadRequest("Invalid email or password.");
            }
            if(!user.Email.Equals(loginRequest.Email) || !user.Password.Equals(loginRequest.Password))
            {
                return BadRequest("Invalid email or password.");
            } 

            return Ok(user);
        }
        [HttpPost("FindUserByEmail")]
        public async Task<ActionResult<User>> FindUserByEmail([FromBody] VerifyEmailViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest("Email is required.");
            }

            try
            {
                var user = await _userRepository.GetUserByEmail(model.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

               
                user.Password = null;

                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound("User not found.");
            }
        }
        [HttpPut("UpdatePassword")]
        public async Task<ActionResult> Update(string email, User editDto)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _userRepository.GetUserByEmail(email)
;

                if (existingItem is null)
                {
                    return NotFound();
                }

                editDto.Email = existingItem.Email;
                var numRecords = await _userRepository.UpdatePassword(email, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();
        }


    }
}

