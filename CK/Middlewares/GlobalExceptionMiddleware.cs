using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CK.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex) when (ex.Message == "IDOR_ATTEMPT")
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { Error = "Bạn không có quyền truy cập dữ liệu của khách hàng khác." });
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (ValidationException ex) // Bắt lỗi Tampering từ FluentValidation
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Error = "Tampering Detected/Dữ liệu không hợp lệ", Details = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (InvalidOperationException ex) // Bắt lỗi Audit Trail
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { Error = "Lỗi hệ thống.", Detail = ex.Message });
        }
    }
}