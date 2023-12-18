using EventsClients.Models;

namespace EventsClient.Models.ViewModels
{
    public class SummaryVM
    {
        public EvHeader Events { get; set; }
        public Booking Bookings { get; set; }
        public BoughtTicket Tickets { get; set; }
        public List<BoughtTicket> ListOfTickets { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CCardNumber { get; set; }
        public string CCardExpDate { get; set; }
        public string CCardCVV { get; set; }
    }
}
