using telemedicine_webapi.Application.Common.Interfaces;
using FluentValidation;

namespace telemedicine_webapi.Application.Hospitals.Commands.CreateHospital;

public class CreateHospitalCommandValidator : AbstractValidator<CreateHospitalCommand>
{
    private readonly IUnitOfWork _context;

    public CreateHospitalCommandValidator(IUnitOfWork context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return await _context.TodoItemRepository.Exists(l => l.Title != title);
    }
}
