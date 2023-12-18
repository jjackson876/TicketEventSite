using EventsAPI.Data;
using EventsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoughtTicketController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BoughtTicketController(AppDbContext context) => _context = context;


        [HttpGet]
        public IActionResult GetAll()
        {
            var item = _context.BoughtTickets.ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpGet("{Id}")]
        public IActionResult GetById(int Id)
        {
            var item = _context.BoughtTickets.ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] BoughtTicket values)
        {
            _context.BoughtTickets.Add(values);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        [HttpPut("{Id}")]
        public IActionResult Update(int Id, [FromBody] BoughtTicket values)
        {
            if (Id != values.Id)
            {
                return NotFound();
            }

            _context.BoughtTickets.Update(values);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var item = _context.BoughtTickets.FirstOrDefault(x => x.Id == Id);
            if (item == null)
            {
                return BadRequest();
            }

            _context.BoughtTickets.Remove(item);
            _context.SaveChanges();

            return Ok(item);
        }
    }
}
