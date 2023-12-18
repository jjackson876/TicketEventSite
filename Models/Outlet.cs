using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventsAPI.Models
{
    public class Outlet
    {
        [Key]
        public int Id { get; set; }
        public string OutletName { get; set; }
        public bool IsDeleted { get; set; }/* = false;*/
        public int? EventListingId { get; set; }

        // relationship
        [ForeignKey("EventListingId")]
        public virtual EvHeader? EventListing { get; set; }
    }
}
