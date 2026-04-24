namespace CK.DTOs;

public class BookingResponse
{
    public string BookingId { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Thông tin loại phòng (Dùng để Frontend biết nên hiển thị icon gì)
    public string RoomType { get; set; } = string.Empty;

    // Các trường đặc thù (Trả về null nếu không có)
    public decimal? CleaningFee { get; set; }
    public string? ButlerName { get; set; }

    // Thông tin nhạy cảm: Chỉ Admin mới được thấy giá trị này
    public string? VipCardNumber { get; set; }

    public bool HasAirportTransfer { get; set; }
    public string? FlightNumber { get; set; }
}