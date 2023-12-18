using EventsClient.Models.DTOs;
using EventsClients.Models;

namespace EventsClient.Models.ViewModels
{
    public class DetailsVM
    {
        public List<int> TicketAmount { get; set; }

        public EvHeader Events { get; set; }
        public List<Admission> Admissions { get; set; }
        public List<Music> Musics { get; set; }
        public List<Outlet> Outlets { get; set; }
        public List<Sponsor> Sponsors { get; set; }
        public Booking Bookings { get; set; }
        public BoughtTicket BoughtTickets { get; set; }
    }
}
