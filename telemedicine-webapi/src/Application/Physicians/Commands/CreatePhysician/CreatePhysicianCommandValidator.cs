using telemedicine_webapi.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Physicians.Commands.CreatePhysician;

public class CreatePhysicianCommandValidator : AbstractValidator<CreatePhysicianCommand>
{
    private readonly IUnitOfWork _context;

    public CreatePhysicianCommandValidator(IUnitOfWork context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(BeUniqueEmail).WithMessage("The specified email already exists.");
    }

    public async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return await _context.PhysicianRepository.Exists(l => l.Email != email);
    }
}
