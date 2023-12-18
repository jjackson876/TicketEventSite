using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsAPI.Models
{
    public class BoughtTicket
    {
        [Key]
        public int Id { get; set; }
        public string TicketType { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }
        public int BookingId { get; set; }

        //relationship
        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }
    }
}
