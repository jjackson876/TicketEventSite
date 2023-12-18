using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EventsClients.Models;

namespace EventsClient.Models
{
    public class Outlet
    {
        public int Id { get; set; }
        public string OutletName { get; set; }
        public bool IsDeleted { get; set; }
        public int? EventListingId { get; set; }

        // relationship
        public EvHeader EventListing { get; set; }
    }
}
