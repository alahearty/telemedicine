using telemedicine_webapi.Application.Common.Interfaces;
using FluentValidation;

namespace telemedicine_webapi.Application.Physicians.Commands.UpdatePhysician;

public class UpdateHospitalCommandValidator : AbstractValidator<UpdatePhysicianCommand>
{
    private readonly IUnitOfWork _context;

    public UpdateHospitalCommandValidator(IUnitOfWork context)
    {
        _context = context;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(BeUniqueEmail).WithMessage("The specified title already exists.");
    }

    public async Task<bool> BeUniqueEmail(UpdatePhysicianCommand model, string title, CancellationToken cancellationToken)
    {
        return await _context.PhysicianRepository.Exists(l => l.Id != model.Id);
    }
}
