using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventsClient.Models.DTOs
{
    public class EvHeaderCreateDTO
    {
        public int Id { get; set; }
        public IFormFile? PromoImage { get; set; }
        public IFormFile? PromoImage2 { get; set; }
        public IFormFile? PromoImage3 { get; set; }

        public string? PromoImgString { get; set; }
        public string? PromoImgString2 { get; set; }
        public string? PromoImgString3 { get; set; }
        public string? PermitImgString { get; set; }

        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDesc { get; set; }
        public IFormFile? Permit { get; set; }
        public int? CategoryId { get; set; }

        // Dropdownlist
        public List<SelectListItem> CategoryList { get; set; }

        //Selected Property
        public int SelectedCategoryId { get; set; }


        public List<Sponsor>? Sponsor { get; set; }
        public List<Admission>? Admission { get; set; }
        public List<Outlet>? Outlet { get; set; }
        public List<Music>? Music { get; set; }
    }
}
