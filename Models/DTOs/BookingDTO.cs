using System.ComponentModel.DataAnnotations.Schema;

namespace EventsAPI.Models.DTOs
{
    public class BookingDTO
    {
        public int? EventListingId { get; set; }
        public virtual List<BoughtTicket>? BoughtTicket { get; set; } = new List<BoughtTicket>();

        //relationship
        [ForeignKey("EventListingId")]
        public virtual EvHeader? EvHeader { get; set; }
    }
}
