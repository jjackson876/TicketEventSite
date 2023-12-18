using EventsAPI.Data;
using EventsAPI.Models;
using EventsAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BookingController(AppDbContext context) => _context = context;


        [HttpGet]
        public IActionResult GetAll()
        {
            var item = _context.Bookings.ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpGet("{Id}")]
        public IActionResult GetById(int Id)
        {
            var item = _context.Bookings.ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Booking values)
        {
            _context.Bookings.Add(values);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromForm] BookingDTO values)
        //{
        //    var booking = new Booking
        //    {
        //        EventListingId = values.EventListingId,
        //    };

        //    await _context.Bookings.AddAsync(booking);
        //    await _context.SaveChangesAsync();

        //    //return Ok(new { Id = booking.Id });
        //    return CreatedAtAction(nameof(GetById), new { Id = booking.Id }, booking);
        //}

        [HttpPut("{Id}")]
        public IActionResult Update(int Id, [FromBody] Booking values)
        {
            if (Id != values.Id)
            {
                return NotFound();
            }

            _context.Bookings.Update(values);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var item = _context.Bookings
                .FirstOrDefault(x => x.Id == Id);


            if (item == null)
            {
                return BadRequest();
            }

            // to delete an event, the properties dependent on the event in other tables must be deleted first

            // Find and remove associated records from tickets table
            var TicketsToDelete = _context.BoughtTickets.Where(a => a.BookingId == Id).ToList();
            _context.BoughtTickets.RemoveRange(TicketsToDelete);

            _context.Bookings.Remove(item);
            _context.SaveChanges();

            return Ok(item);
        }
    }
}
