using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repository;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly ITicketRepository _ticketRepository;

        

        public TicketController(ILogger<TicketController> logger, ITicketRepository ticketRepository)
        {

            _logger = logger;
            _ticketRepository = ticketRepository;
        }

        [HttpGet("GetAllTickets")]
        public async Task<ActionResult<Ticket>> GetAll()
        {
            return Ok(await _ticketRepository.GetAllTickets());
        }

     
        [HttpGet("GetTicketById")]
        public async Task<Ticket> GetById(int id)
        {
            return await _ticketRepository.GetTicketById(id)
;
        }



      
        [HttpPost("Insert")]
        public async Task<ActionResult> Post( Ticket newDto)
        {
            if (ModelState.IsValid)
            {
              
                var newId = await _ticketRepository.InsertTicket(newDto);

                return Ok(newId);
            }
            else return BadRequest();
        }

     
        [HttpPut("Update")]
        public async Task<ActionResult> Put(int id,  Ticket editDto)
        {
            if (ModelState.IsValid)
            {
                var existingItem = await _ticketRepository.GetTicketById(id)
;

                if (existingItem is null)
                {
                    return NotFound();
                }

                editDto.Id = existingItem.Id;
                var numRecords = await _ticketRepository.UpdateTicket(id, editDto);

                return Ok(numRecords);
            }
            else return BadRequest();
        }

    
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete( int key)
        {

            var existingItem = await _ticketRepository.GetTicketById(key)
;

            if (existingItem is null)
            {
                return NotFound();
            }
            try
            {
                var numRecords = await _ticketRepository.DeleteTicket(key)
;

                return Ok(numRecords);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
