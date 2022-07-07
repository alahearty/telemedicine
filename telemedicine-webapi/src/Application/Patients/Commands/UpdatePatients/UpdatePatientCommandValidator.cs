using telemedicine_webapi.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Patients.Commands.UpdatePatients;

public class UpdateHospitalCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateHospitalCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
    }

    public async Task<bool> BeUniqueTitle(UpdatePatientCommand model, string name, CancellationToken cancellationToken)
    {
        return await _context.Patients
            .Where(l => l.Id != model.Id)
            .AllAsync(l => l.FirstName != name, cancellationToken);
    }
}
