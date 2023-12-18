using System.ComponentModel.DataAnnotations.Schema;

namespace EventsClient.Models
{
    public class BoughtTicket
    {
        public int Id { get; set; }
        public string TicketType { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }
        public int BookingId { get; set; }

        //navigation
        public Booking Booking { get; set; }
    }
}
