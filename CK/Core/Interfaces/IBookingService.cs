using CK.Core.Entities;

namespace CK.Core.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<Booking>> GetBookingsAsync(int currentUserId, string role);
    Task<Booking> GetBookingByIdAsync(string bookingId, int currentUserId, string role);
    Task CreateBookingAsync(Booking booking, int currentUserId);
    Task UpdateBookingNotesAsync(string bookingId, string notes, int currentUserId);
    Task UpdateBookingStatusAdminAsync(string bookingId, string status, string? butlerName);
    Task SoftDeleteBookingAsync(string bookingId, string role);
}