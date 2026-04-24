using CK.Core.Entities;
using CK.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CK.DTOs;

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

    private BookingResponse MapToResponse(Booking entity, string role)
    {
        var response = new BookingResponse
        {
            BookingId = entity.BookingId,
            CheckInDate = entity.CheckInDate,
            Amount = entity.Amount,
            Status = entity.Status,
            Notes = entity.Notes,
            HasAirportTransfer = entity.HasAirportTransfer,
            FlightNumber = entity.FlightNumber
        };

        if (entity is StandardBooking standard)
        {
            response.RoomType = "Standard";
            response.CleaningFee = standard.CleaningFee;
        }
        else if (entity is SuiteBooking suite)
        {
            response.RoomType = "Suite";
            response.ButlerName = suite.ButlerName;

            // BẢO MẬT: Chỉ Admin mới được map trường thông tin nhạy cảm
            if (role == "Admin")
            {
                response.VipCardNumber = suite.VipCardNumber;
            }
        }
        return response;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var bookings = await _bookingService.GetBookingsAsync(CurrentUserId, CurrentRole);
        var response = bookings.Select(b => MapToResponse(b, CurrentRole));
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(string id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id, CurrentUserId, CurrentRole);
            return Ok(MapToResponse(booking, CurrentRole));
        }
        catch (UnauthorizedAccessException) // Lỗi IDOR
        {
            return Forbid(); // Trả về 403 Forbidden theo đúng yêu cầu đề bài
        }
    }

    [HttpPut("{id}/admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAdmin(string id, [FromBody] AdminUpdateBookingRequest request)
    {
        // Gọi xuống hàm đã được viết sẵn trong Service
        await _bookingService.UpdateBookingStatusAdminAsync(id, request.Status, request.ButlerName);
        return NoContent();
    }


    [HttpPost("standard")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateStandard([FromBody] StandardBooking booking)
    {
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
    public async Task<IActionResult> UpdateNotes(string id, [FromBody] UpdateNoteRequest request)
    {
        await _bookingService.UpdateBookingNotesAsync(id, request.Notes, CurrentUserId);
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