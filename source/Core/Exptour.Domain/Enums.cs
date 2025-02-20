using System.ComponentModel;

namespace Exptour.Domain;

public enum Currency
{
    [Description("USD")] USD,
    [Description("AED")] AED,
    [Description("EUR")] EUR
}

public enum Gender
{
    [Description("Male")] Male,
    [Description("Female")] Female,
    [Description("Unknown")] RatherNotSay,
}

public enum TourStatus
{
    [Description("Active")] Active,
    [Description("Cancelled")] Cancelled,
    [Description("Ended")] Ended,
}

public enum TimeOfDay
{
    [Description("Starts before 12pm")] Morning,
    [Description("Starts after 12pm")] Afternoon,
    [Description("Starts after 5pm")] EveningAndNight
}

public enum RoomType
{
    [Description("Single")] Single,
    [Description("Couple")] Couple,
    [Description("Suite")] Suite
}

public enum AvailabilityStatus
{
    [Description("The hotel is available")] Available,
    [Description("The hotel is fully booked")] FullyBooked,
    [Description("The hotel is under maintenance and temporarily closed")] UnderMaintenance,
    [Description("The hotel is open only during specific seasons")] Seasonal,
    [Description("The hotel is permanently closed")] Closed,
    [Description("The hotel is accessible only for specific tours or guests")] Restricted
}

public enum BookingStatus
{
    [Description("Pending")] Pending,
    [Description("Confirmed")] Confirmed,
    [Description("Cancelled")] Cancelled
}

public enum PaymentStatus
{
    [Description("Pending")] Pending,
    [Description("Paid")] Paid,
    [Description("Failed")] Failed,
    [Description("Refunded")] Refunded
}

/// <summary>
/// Represents the available payment methods.
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Payment using a credit or debit card.
    /// </summary>
    [Description("Card")] Card,
    /// <summary>
    /// Payment using cash.
    /// </summary>
    [Description("Cash")] Cash,
    /// <summary>
    /// Buy now, pay later payment option.
    /// </summary>
    [Description("BuyNowPayLater")] BuyNowPayLater,
    /// <summary>
    /// Payment via digital wallets such as Apple Pay or Google Pay.
    /// </summary>
    [Description("DigitalWallet")] DigitalWallet,
    /// <summary>
    /// Payment through bank transfer, including wire transfer or EFT.
    /// </summary>
    [Description("BankTransfer")] BankTransfer,
    /// <summary>
    /// Payment using cryptocurrencies like Bitcoin or Ethereum.
    /// </summary>
    [Description("Crypto")] Crypto
}

public enum OfferStatus
{
    [Description("Pending")] Pending,
    [Description("Approved")] Approved,
    [Description("Rejected")] Rejected
}

public enum CarStatus
{
    [Description("Available")] Available,
    [Description("Reserved")] Reserved,
    [Description("InUse")] InUse,
    [Description("Maintenance")] Maintenance
}

public enum ImageType
{
    [Description("Banner")] Banner,
    [Description("Gallery")] Gallery,
    [Description("Thumbnail")] Thumbnail
}
