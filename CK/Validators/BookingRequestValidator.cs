using CK.Core.Entities;
using FluentValidation;

namespace CK.Validators;

public class StandardBookingValidator : AbstractValidator<StandardBooking>
{
    public StandardBookingValidator()
    {
        // Chống Tampering số tiền âm
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithMessage("Số tiền không được phép nhỏ hơn 0.");

        // Validation dịch vụ đưa đón
        RuleFor(x => x.FlightNumber)
            .NotEmpty().When(x => x.HasAirportTransfer)
            .WithMessage("Phải cung cấp số hiệu chuyến bay nếu chọn dịch vụ đưa đón.");
    }
}

public class SuiteBookingValidator : AbstractValidator<SuiteBooking>
{
    public SuiteBookingValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithMessage("Số tiền không được phép nhỏ hơn 0.");

        RuleFor(x => x.FlightNumber)
            .NotEmpty().When(x => x.HasAirportTransfer)
            .WithMessage("Phải cung cấp số hiệu chuyến bay nếu chọn dịch vụ đưa đón.");

        // Bắt buộc phòng Suite phải có tên Quản gia (Tampering check)
        RuleFor(x => x.ButlerName)
            .NotEmpty().WithMessage("Phòng Suite bắt buộc phải có tên Quản gia.");
    }
}