using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventsAPI.Models
{
    public class Sponsor
    {
        [Key]
        public int Id { get; set; }
        public string EventSponsor { get; set; }
        public bool IsDeleted { get; set; }/* = false;*/
        public int? EventListingId { get; set; }

        // relationship
        [ForeignKey("EventListingId")]
        public virtual EvHeader? EventListing { get; set; }
    }
}
