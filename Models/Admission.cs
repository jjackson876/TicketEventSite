using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventsAPI.Models
{
    public class Admission
    {
        [Key]
        public int Id { get; set; }
        public string AdmissionType { get; set; }
        public double Price { get; set; }
        public int? Quantity { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsNotPurchasable { get; set; } = false;
        public int? EventListingId { get; set; } 

        // relationship
        [ForeignKey("EventListingId")]
        public virtual EvHeader? EvHeader { get; set; }
    }
}
