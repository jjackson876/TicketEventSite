using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EventsAPI.Models
{
    public class EvHeader
    {
        [Key]
        public int Id { get; set; }
        public string? PromoImage { get; set; } = String.Empty;
        public string? PromoImage2 { get; set; } = String.Empty;
        public string? PromoImage3 { get; set; } = String.Empty;
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDesc { get; set; }
        public string? Permit { get; set; } = String.Empty;
        public int? CategoryId { get; set; }

        // relationship
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [JsonIgnore]
        public virtual List<Music>? Music { get; set; } = new List<Music>();
        [JsonIgnore]
        public virtual List<Sponsor>? Sponsor { get; set; } = new List<Sponsor>();
        [JsonIgnore]
        public virtual List<Admission>? Admissions { get; set; } = new List<Admission>();
        [JsonIgnore]
        public virtual List<Outlet>? Outlet { get; set; } = new List<Outlet>();
    }
}
