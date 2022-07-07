﻿using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Hospitals.Commands.DeleteHospital;

public record DeleteHospitalCommand(int Id) : IRequest;

public class DeleteHospitalCommandHandler : IRequestHandler<DeleteHospitalCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteHospitalCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoList), request.Id);
        }

        _context.TodoLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
