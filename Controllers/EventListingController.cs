using EventsAPI.Data;
using EventsAPI.Models;
using EventsAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EventsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventListingController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EventListingController(AppDbContext context) => _context = context;


        [HttpGet]
        public IActionResult GetAll()
        {
            var item = _context.Events
                .ToList();

            if (item == null)
            {
                return BadRequest();
            }

            // construct the full image url for each eventlisting
            var baseUrl = "https://localhost:7161/images/";

            // generate the full file path for each listing
            foreach (var listing in item)
            {
                listing.PromoImage = baseUrl + listing.PromoImage;
                listing.PromoImage2 = baseUrl + listing.PromoImage2;
                listing.PromoImage3 = baseUrl + listing.PromoImage3;
                listing.Permit = baseUrl + listing.Permit;
            }

            return Ok(item);
        }

        [HttpGet("{Id}")]
        public IActionResult GetById(int Id)
        {
            var item = _context.Events
                //.Include(b => b.Admissions)
                //.Include(b => b.Music)
                //.Include(b => b.Outlet)
                //.Include(b => b.Sponsor)
                .ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EvHeaderCreateDTO values)
        {

            var promoImage = values.PromoImage;
            var promoImage2 = values.PromoImage2;
            var promoImage3 = values.PromoImage3;
            var permit = values.Permit;

            if (promoImage != null && promoImage.Length > 0
                && promoImage2 != null && promoImage2.Length > 0
                && promoImage3 != null && promoImage3.Length > 0
                && permit != null && permit.Length > 0)
            {
                // generate a unique file name
                var uPromo1 = Guid.NewGuid() + "_" + promoImage.FileName;
                var uPromo2 = Guid.NewGuid() + "_" + promoImage2.FileName;
                var uPromo3 = Guid.NewGuid() + "_" + promoImage3.FileName;
                var uPermit = Guid.NewGuid() + "_" + permit.FileName;

                // define the final file path on the api server
                var apiFilePath = Path.Combine("api", "server", "uploads", uPromo1);
                var apiFilePath2 = Path.Combine("api", "server", "uploads", uPromo2);
                var apiFilePath3 = Path.Combine("api", "server", "uploads", uPromo3);
                var apiFilePath4 = Path.Combine("api", "server", "uploads", uPermit);

                // save the file to server
                using (var stream = new FileStream(apiFilePath, FileMode.Create))
                {
                    await promoImage.CopyToAsync(stream);
                }

                using (var stream2 = new FileStream(apiFilePath2, FileMode.Create))
                {
                    await promoImage2.CopyToAsync(stream2);
                }

                using (var stream3 = new FileStream(apiFilePath3, FileMode.Create))
                {
                    await promoImage3.CopyToAsync(stream3);
                }

                using (var stream4 = new FileStream(apiFilePath4, FileMode.Create))
                {
                    await permit.CopyToAsync(stream4);
                }

                //store the file path in the database along with other listing details
                var listing = new EvHeader
                {
                    PromoImage = apiFilePath != String.Empty ? apiFilePath : "",
                    PromoImage2 = apiFilePath2 != String.Empty ? apiFilePath2 : "",
                    PromoImage3 = apiFilePath3 != String.Empty ? apiFilePath3 : "",
                    EventName = values.EventName,
                    EventLocation = values.EventLocation,
                    EventDate = values.EventDate,
                    EventDesc = values.EventDesc,
                    CategoryId = values.CategoryId,
                    Permit = apiFilePath4 != String.Empty ? apiFilePath4 : ""
                };

                _context.Events.Add(listing);
                await _context.SaveChangesAsync();

                return Ok(new { Id = listing.Id });
                //return CreatedAtAction(nameof(GetById), new { Id = listing.Id }, values);
            }

            return BadRequest();
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateAsync(int Id, [FromForm] EvHeaderCreateDTO values)
        {
            if (values == null || Id != values.Id)
            {
                return NotFound();
            }

            var PromoImage = values.PromoImage;
            var PromoImage2 = values.PromoImage2;
            var PromoImage3 = values.PromoImage3;
            var Permit = values.Permit;

            // Check if image is provided
            if (PromoImage != null && PromoImage.Length > 0
                && PromoImage2 != null && PromoImage2.Length > 0
                && PromoImage3 != null && PromoImage3.Length > 0
                && Permit != null && Permit.Length > 0)
            {
                // Unique file name for the front image
                var uPromo1 = Guid.NewGuid() + "_" + PromoImage.FileName;
                var uPromo2 = Guid.NewGuid() + "_" + PromoImage2.FileName;
                var uPromo3 = Guid.NewGuid() + "_" + PromoImage3.FileName;
                var uPermit = Guid.NewGuid() + "_" + Permit.FileName;

                // Define the final file path
                var apiFilePath = Path.Combine("api", "server", "uploads", uPromo1);
                var apiFilePath2 = Path.Combine("api", "server", "uploads", uPromo2);
                var apiFilePath3 = Path.Combine("api", "server", "uploads", uPromo3);
                var apiFilePath4 = Path.Combine("api", "server", "uploads", uPermit);

                // Save the new front image to the server

                using (var stream = new FileStream(apiFilePath, FileMode.Create))
                {
                    await PromoImage.CopyToAsync(stream);
                }

                using (var stream2 = new FileStream(apiFilePath2, FileMode.Create))
                {
                    await PromoImage2.CopyToAsync(stream2);
                }

                using (var stream3 = new FileStream(apiFilePath3, FileMode.Create))
                {
                    await PromoImage3.CopyToAsync(stream3);
                }

                using (var stream4 = new FileStream(apiFilePath4, FileMode.Create))
                {
                    await Permit.CopyToAsync(stream4);
                }

                var listing = new EvHeader
                {
                    Id = values.Id,
                    PromoImage = string.IsNullOrWhiteSpace(apiFilePath) ? "" : apiFilePath,
                    PromoImage2 = string.IsNullOrWhiteSpace(apiFilePath2) ? "" : apiFilePath2,
                    PromoImage3 = string.IsNullOrWhiteSpace(apiFilePath3) ? "" : apiFilePath3,
                    Permit = string.IsNullOrWhiteSpace(apiFilePath4) ? "" : apiFilePath4,
                    EventName = values.EventName,
                    EventLocation = values.EventLocation,
                    EventDate = values.EventDate,
                    EventDesc = values.EventDesc,
                    CategoryId = values.CategoryId,
                };

                _context.Events.Update(listing);
                await _context.SaveChangesAsync();

                return Ok(new { Id = listing.Id });
            }
            else if (PromoImage == null && PromoImage2 == null && PromoImage3 == null && Permit == null)
            {
                var listing2 = new EvHeader
                {
                    Id = values.Id,
                    EventName = values.EventName,
                    EventLocation = values.EventLocation,
                    EventDate = values.EventDate,
                    EventDesc = values.EventDesc,
                    CategoryId = values.CategoryId,
                };

                _context.Events.Update(listing2);
                await _context.SaveChangesAsync();

                return Ok(new { Id = listing2.Id });
            }
            return BadRequest();
        }

        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            var item = _context.Events
                .Include(x => x.Admissions)
                .Include(x => x.Music)
                .Include(x => x.Outlet)
                .Include(x => x.Sponsor)
                .FirstOrDefault(x => x.Id == Id);

            if (item == null)
            {
                return BadRequest();
            }

            // to delete an event, the properties dependent on the event in other tables must be deleted first

            // Find and remove associated records from Admissions table
            var admissionsToDelete = _context.Admissions.Where(a => a.EventListingId == Id).ToList();
            _context.Admissions.RemoveRange(admissionsToDelete);

            // Find and remove associated records from Music table
            var musicToDelete = _context.Musics.Where(m => m.EventListingId == Id).ToList();
            _context.Musics.RemoveRange(musicToDelete);

            // Find and remove associated records from Outlet table
            var outletToDelete = _context.Outlets.Where(o => o.EventListingId == Id).ToList();
            _context.Outlets.RemoveRange(outletToDelete);

            // Find and remove associated records from Sponsor table
            var sponsorToDelete = _context.Sponsors.Where(s => s.EventListingId == Id).ToList();
            _context.Sponsors.RemoveRange(sponsorToDelete);

            _context.Events.Remove(item);
            _context.SaveChanges();

            return Ok(item);
        }

        [HttpGet("files/{filename}")]
        public IActionResult GetFile(string fileName)
        {
            // construct the full path to the file based on the provided filename
            string filePath = Path.Combine("api", "server", "uploads", fileName);

            //check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            // Determine the content type based on the file's extension
            string contentType = GetContentType(fileName);

            // Return the image file as a FileStreamResult with the appropriate content type
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, contentType); // Adjust the content type as needed
        }

        private string GetContentType(string fileName)
        {
            // Determine the content type based on the file's extension
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".pdf":
                    return "application/pdf";
                default:
                    return "application/octet-stream"; // Default to binary data
            }
        }
    }
}
