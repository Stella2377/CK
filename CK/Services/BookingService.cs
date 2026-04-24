using CK.Core.Entities;
using CK.Core.Interfaces;

namespace CK.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Booking>> GetBookingsAsync(int currentUserId, string role)
    {
        if (role == "Admin")
        {
            return await _repository.GetAllAsync(includeSoftDeleted: true); // Admin thấy tất cả, kể cả xóa mềm
        }
        return await _repository.GetByUserIdAsync(currentUserId); // User chỉ thấy của mình
    }

    public async Task<Booking> GetBookingByIdAsync(string bookingId, int currentUserId, string role)
    {
        var booking = await _repository.GetByIdAsync(bookingId);
        if (booking == null) throw new KeyNotFoundException("Không tìm thấy đơn đặt phòng.");

        // BẢO MẬT: Ngăn chặn IDOR
        if (role != "Admin" && booking.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("IDOR_ATTEMPT");
        }

        return booking;
    }

    public async Task CreateBookingAsync(Booking booking, int currentUserId)
    {
        booking.UserId = currentUserId;
        booking.Status = "Pending";
        await _repository.AddAsync(booking);
    }

    public async Task UpdateBookingNotesAsync(string bookingId, string notes, int currentUserId)
    {
        var booking = await GetBookingByIdAsync(bookingId, currentUserId, "User");

        // AUDIT TRAIL: Không cho phép sửa nếu đã thanh toán hoặc trả phòng
        if (booking.Status == "Paid" || booking.Status == "CheckedOut")
        {
            throw new InvalidOperationException("Không thể chỉnh sửa đơn đã thanh toán/trả phòng.");
        }

        booking.Notes = notes;
        await _repository.UpdateAsync(booking);
    }

    public async Task UpdateBookingStatusAdminAsync(string bookingId, string status, string? butlerName)
    {
        var booking = await _repository.GetByIdAsync(bookingId);
        if (booking == null) throw new KeyNotFoundException();

        booking.Status = status;
        if (booking is SuiteBooking suite && !string.IsNullOrEmpty(butlerName))
        {
            suite.ButlerName = butlerName;
        }

        await _repository.UpdateAsync(booking);
    }

    public async Task SoftDeleteBookingAsync(string bookingId, string role)
    {
        if (role != "Admin") throw new UnauthorizedAccessException();
        await _repository.SoftDeleteAsync(bookingId);
    }
}