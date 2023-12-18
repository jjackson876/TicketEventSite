using EventsClient.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsClients.Models
{
    public class EvHeader
    {
        public int Id { get; set; }
        public string? PromoImage { get; set; }
        public string? PromoImage2 { get; set; }
        public string? PromoImage3 { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDesc { get; set; }
        public string? Permit { get; set; }
        public int? CategoryId { get; set; }

        // relationship
        public virtual Category? Category { get; set; }

        public List<Music> Music { get; set; }
        public List<Sponsor> Sponsor { get; set; }
        public List<Admission> Admission { get; set; }
        public List<Outlet> Outlet { get; set; }
    }
}
