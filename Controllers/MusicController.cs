using EventsAPI.Data;
using EventsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MusicController(AppDbContext context) => _context = context;


        [HttpGet]
        public IActionResult GetAll()
        {
            var item = _context.Musics.ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpGet("{Id}")]
        public IActionResult GetById(int Id)
        {
            var item = _context.Musics
                .Include(b => b.EventListing)
                .ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Music values)
        {
            _context.Musics.Add(values);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] Music values)
        {
            var existingEntity = await _context.Musics.FindAsync(Id);

            if (existingEntity == null)
            {
                // If not found, try finding by EventListingId alone
                existingEntity = await _context.Musics.FirstOrDefaultAsync(a => a.EventListingId == Id);

                if (existingEntity == null)
                {
                    return NotFound();
                }
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(new
            {
                values.MusicProvider,
                values.IsDeleted,
                values.EventListingId,
            });

            await _context.SaveChangesAsync();

            return Ok(existingEntity);
        }

        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var item = _context.Musics.FirstOrDefault(x => x.Id == Id);
            if (item == null)
            {
                return BadRequest();
            }

            _context.Musics.Remove(item);
            _context.SaveChanges();

            return Ok(item);
        }
    }
}
