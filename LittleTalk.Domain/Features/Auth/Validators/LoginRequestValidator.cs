using LittleTalk.Shared;
using LittleTalk.Models.Common;
using FluentValidation;
using LittleTalk.Domain.Features.Auth.DTOs;

namespace LittleTalk.Domain.Features.Auth.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().WithMessage("Username or Email is required.")
            .MaximumLength(100).WithMessage("Username or Email must not exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}


