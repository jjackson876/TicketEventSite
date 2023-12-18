using EventsAPI.Data;
using EventsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context) => _context = context;


        [HttpGet]
        //[AllowAnonymous]
        //[Authorize(Roles = "admin")]
        public IActionResult GetAll()
        {
            var item = _context.Categories.ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpGet("{Id}")]
        //[Authorize(Roles = "admin")]
        //[AllowAnonymous]
        public IActionResult GetById(int Id)
        {
            var item = _context.Categories.ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        public IActionResult Create([FromBody] Category values)
        {
            _context.Categories.Add(values);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        [HttpPut("{Id}")]
        //[Authorize(Roles = "admin")]
        public IActionResult Update(int Id, [FromBody] Category values)
        {
            if (Id != values.Id)
            {
                return NotFound();
            }

            _context.Categories.Update(values);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { Id = values.Id }, values);
        }

        [HttpDelete("{Id}")]
        //[Authorize(Roles = "admin")]
        public IActionResult Delete(int Id)
        {
            var item = _context.Categories.FirstOrDefault(x => x.Id == Id);
            if (item == null)
            {
                return BadRequest();
            }

            _context.Categories.Remove(item);
            _context.SaveChanges();

            return Ok(item);
        }
    }
}
