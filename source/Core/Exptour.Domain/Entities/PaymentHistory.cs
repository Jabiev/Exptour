using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class PaymentHistory : BaseEntity
{
    public Guid PaymentId { get; set; }
    public PaymentStatus Status { get; set; } // status of payment process
    public string? Notes { get; set; }  //additional informations about payment

    //RELATIONSHIPs
    public Payment Payment { get; set; }
}
