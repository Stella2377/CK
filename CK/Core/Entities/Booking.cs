namespace CK.Core.Entities;

public abstract class Booking
{
    // Bắt buộc khởi tạo và bất biến (Immutability)
    public required string BookingId { get; init; }
    public required DateTime CheckInDate { get; init; }

    public int UserId { get; set; } // Khóa ngoại liên kết với User
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, CheckedIn, CheckedOut, SoftDeleted
    public string? Notes { get; set; }
    public bool HasAirportTransfer { get; set; }
    public string? FlightNumber { get; set; } // Bắt buộc nếu HasAirportTransfer = true
}