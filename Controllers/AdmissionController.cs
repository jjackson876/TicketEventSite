using EventsAPI.Data;
using EventsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EventsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdmissionController(AppDbContext context) => _context = context;


        [HttpGet]
        public IActionResult GetAll()
        {
            var item = _context.Admissions
                .ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpGet("{Id}")]
        public IActionResult GetById(int Id)
        {
            var item = _context.Admissions
                .Include(b => b.EvHeader)
                .ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Admission values)
        {
            await _context.Admissions.AddAsync(values);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] Admission values)
        {
            var existingEntity = await _context.Admissions.FindAsync(Id);

            if (existingEntity == null)
            {
                // If not found, try finding by EventListingId alone
                existingEntity = await _context.Admissions.FirstOrDefaultAsync(a => a.EventListingId == Id);

                if (existingEntity == null)
                {
                    return NotFound();
                }
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(new
            {
                values.AdmissionType,
                values.Price,
                values.IsDeleted,
                values.EventListingId,
                values.Quantity,
                values.IsNotPurchasable
            });

            await _context.SaveChangesAsync();

            return Ok(existingEntity);
        }

        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var item = _context.Admissions.FirstOrDefault(x => x.Id == Id);
            if (item == null)
            {
                return BadRequest();
            }

            _context.Admissions.Remove(item);
            _context.SaveChanges();

            return Ok(item);
        }
    }
}
