namespace EventsAPI.Models.DTOs
{
    public class EvHeaderCreateDTO
    {
        public int Id { get; set; }
        public IFormFile? PromoImage { get; set; }
        public IFormFile? PromoImage2 { get; set; }
        public IFormFile? PromoImage3 { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDesc { get; set; }
        public IFormFile? Permit { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public virtual List<Music>? Music { get; set; } = new List<Music>();
        public virtual List<Sponsor>? Sponsor { get; set; } = new List<Sponsor>();
        public virtual List<Admission>? Admission { get; set; } = new List<Admission>();
        public virtual List<Outlet>? Outlet { get; set; } = new List<Outlet>();
    }
}
