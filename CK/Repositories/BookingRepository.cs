using CK.Core.Entities;
using CK.Core.Interfaces;
using CK.Data;
using Microsoft.EntityFrameworkCore;

namespace CK.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(bool includeSoftDeleted = false)
    {
        // Admin có quyền xem cả hóa đơn đã xóa mềm (IgnoreQueryFilters)
        if (includeSoftDeleted)
        {
            return await _context.Bookings.IgnoreQueryFilters().ToListAsync();
        }
        return await _context.Bookings.ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
    {
        return await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(string bookingId)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(string bookingId)
    {
        var booking = await GetByIdAsync(bookingId);
        if (booking != null)
        {
            booking.Status = "SoftDeleted";
            await _context.SaveChangesAsync();
        }
    }
}