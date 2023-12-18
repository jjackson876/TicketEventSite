using EventsClients.Models;
using System.ComponentModel.DataAnnotations;

namespace EventsClient.Models
{
    public class Music
    {
        public int Id { get; set; }
        public string MusicProvider { get; set; }
        public bool IsDeleted { get; set; }
        public int? EventListingId { get; set; }

        // relationship
        public EvHeader EventListing { get; set; }
    }
}
