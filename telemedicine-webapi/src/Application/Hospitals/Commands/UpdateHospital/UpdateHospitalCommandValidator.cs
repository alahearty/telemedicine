using telemedicine_webapi.Application.Common.Interfaces;
using FluentValidation;

namespace telemedicine_webapi.Application.Hospitals.Commands.UpdateHospital;

public class UpdateHospitalCommandValidator : AbstractValidator<UpdateHospitalCommand>
{
    private readonly IUnitOfWork _context;

    public UpdateHospitalCommandValidator(IUnitOfWork context)
    {
        _context = context;

        RuleFor(v => v.HospitalName)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
    }

    public async Task<bool> BeUniqueTitle(UpdateHospitalCommand model, string title, CancellationToken cancellationToken)
    {
        return await _context.TodoListRepository.Exists(l => l.Title != title);
    }
}
