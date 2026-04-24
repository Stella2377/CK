namespace CK.DTOs;

// Lớp cơ sở cho các yêu cầu đặt phòng
public abstract class BaseBookingRequest
{
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public bool HasAirportTransfer { get; set; }
    public string? FlightNumber { get; set; }
}

// Request dành cho phòng Standard
public class StandardBookingRequest : BaseBookingRequest
{
    public decimal CleaningFee { get; set; } = 150000;
}

// Request dành cho phòng Suite
public class SuiteBookingRequest : BaseBookingRequest
{
    public string? ButlerName { get; set; }
    public string? VipCardNumber { get; set; }
}