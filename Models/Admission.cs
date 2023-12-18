using EventsClient.Models;
using EventsClients.Models;

namespace EventsClient.Models
{
    public class Admission
    {
        public int? Id { get; set; }
        public string AdmissionType { get; set; }
        public double Price { get; set; }
        public int? Quantity { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsNotPurchasable { get; set; }
        public int? EventListingId { get; set; }

        // navigation
        public EvHeader EvHeader { get; set; }
    }
}
