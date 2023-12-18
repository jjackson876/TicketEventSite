using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? EventListingId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        [DisplayName("Credit Card Number")]
        public string? CCardNumber { get; set; }

        [DisplayName("Expiry Date")]
        public string CCardExpDate { get; set; }

        [DisplayName("CVV")]
        public string? CCardCVV { get; set; }

        //relationship
        [ForeignKey("EventListingId")]
        public virtual EvHeader? EvHeader { get; set; }
    }
}
