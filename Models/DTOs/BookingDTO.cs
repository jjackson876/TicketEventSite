using EventsClients.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsClient.Models.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int? EventListingId { get; set; }
        public List<BoughtTicket>? BoughtTicket { get; set; }

        //relationship
        public EvHeader? EvHeader { get; set; }
    }
}
