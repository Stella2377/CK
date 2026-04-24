using CK.Core.Entities;

namespace CK.Core.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync(bool includeSoftDeleted = false);
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
    Task<Booking?> GetByIdAsync(string bookingId);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task SoftDeleteAsync(string bookingId);
}