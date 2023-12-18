using EventsClients.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsClient.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? EventListingId { get; set; }
        //public List<BoughtTicket>? BoughtTicket { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CCardNumber { get; set; }
        public string? CCardExpDate { get; set; }
        public string? CCardCVV { get; set; }


        //navigation
        public EvHeader EvHeader { get; set; }

        //public double FindSubTotal(int ticketPrice, int ticketQuantity)
        //{
        //    var subtotal = ticketPrice * ticketQuantity;

        //    return subtotal;
        //}
    }
}

