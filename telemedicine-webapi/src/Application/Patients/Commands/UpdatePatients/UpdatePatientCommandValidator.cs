using telemedicine_webapi.Application.Common.Interfaces;
using FluentValidation;

namespace telemedicine_webapi.Application.Patients.Commands.UpdatePatients;

public class UpdateHospitalCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    private readonly IUnitOfWork _context;

    public UpdateHospitalCommandValidator(IUnitOfWork context)
    {
        _context = context;

        //RuleFor(v => v.Name)
        //    .NotEmpty().WithMessage("Title is required.")
        //    .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
        //    .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
    }

    public async Task<bool> BeUniqueTitle(UpdatePatientCommand model, string name, CancellationToken cancellationToken)
    {
        return await _context.PatientRepository.Exists(l => l.Id != model.Id);
    }
}
