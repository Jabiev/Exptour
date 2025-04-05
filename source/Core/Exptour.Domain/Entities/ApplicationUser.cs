using Exptour.Common.Helpers;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Exptour.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [StringLength(100, MinimumLength = 2)]
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }

    [StringLength(50)]
    [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
    public string? NationalityEN { get; set; }

    [StringLength(50)]
    public string? NationalityAR { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string FullName => $"{UserName?.ToSentenceCase()} {LastName?.ToSentenceCase()}";

    //RELATIONSHIPs
    public ICollection<Hotel> Hotels { get; set; }
    public ICollection<Tour> Tours { get; set; }
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Booking> Bookings { get; set; }
}
