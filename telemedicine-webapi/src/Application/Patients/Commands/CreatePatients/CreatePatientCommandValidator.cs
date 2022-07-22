using telemedicine_webapi.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Patients.Commands.CreatePatients;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    private readonly IUnitOfWork _context;

    public CreatePatientCommandValidator(IUnitOfWork context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return await _context.TodoListRepository.Exists(l => l.Title != title);
    }
}
