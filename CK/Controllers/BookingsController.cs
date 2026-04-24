using CK.Core.Entities;
using CK.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CK.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentRole => User.FindFirstValue(ClaimTypes.Role)!;

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var bookings = await _bookingService.GetBookingsAsync(CurrentUserId, CurrentRole);
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(string id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id, CurrentUserId, CurrentRole);
            return Ok(booking);
        }
        catch (UnauthorizedAccessException) // Lỗi IDOR
        {
            return Forbid(); // Trả về 403 Forbidden theo đúng yêu cầu đề bài
        }
    }

    [HttpPost("standard")]
    [Authorize(Roles = "User")] // Chỉ User mới được tạo
    public async Task<IActionResult> CreateStandard([FromBody] StandardBooking booking)
    {
        // Tampering validation sẽ được FluentValidation tự động bắt và trả về 400 trước khi vào hàm này
        await _bookingService.CreateBookingAsync(booking, CurrentUserId);
        return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
    }

    [HttpPost("suite")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateSuite([FromBody] SuiteBooking booking)
    {
        await _bookingService.CreateBookingAsync(booking, CurrentUserId);
        return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
    }

    [HttpPut("{id}/notes")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateNotes(string id, [FromBody] string notes)
    {
        await _bookingService.UpdateBookingNotesAsync(id, notes, CurrentUserId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SoftDelete(string id)
    {
        await _bookingService.SoftDeleteBookingAsync(id, CurrentRole);
        return NoContent();
    }
}