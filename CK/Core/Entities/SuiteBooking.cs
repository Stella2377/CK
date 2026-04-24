namespace CK.Core.Entities;

public class SuiteBooking : Booking
{
    public string? ButlerName { get; set; } // Quản gia riêng
    public string? VipCardNumber { get; set; }
}