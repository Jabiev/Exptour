using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid BookingId { get; set; }
    public string TransactionId { get; set; } //the Id which being come from services such Stripe as
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTimeOffset ProcessedAt { get; set; }  //completed time of the process

    //RELATIONSHIPs
    public Booking Booking { get; set; }
    public ICollection<PaymentHistory> PaymentHistories { get; set; }
}
