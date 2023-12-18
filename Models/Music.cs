using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsAPI.Models
{
    public class Music
    {
        [Key]
        public int Id { get; set; }
        public string MusicProvider { get; set; }
        public bool IsDeleted { get; set; }/* = false;*/
        public int? EventListingId { get; set; }

        // relationship
        [ForeignKey("EventListingId")]
        public virtual EvHeader? EventListing { get; set; }
    }
}
