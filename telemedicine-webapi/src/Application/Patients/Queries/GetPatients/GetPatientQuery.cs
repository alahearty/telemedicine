using AutoMapper;
using MediatR;
using telemedicine_webapi.Application.Common.Exceptions;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Patients.Queries.GetPatients;

public record GetPatientQuery(int id) : IRequest<BaseResponse>;

public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, BaseResponse>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetPatientQueryHandler(IUnitOfWork context, IMapper mapper, ICurrentUserService currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<BaseResponse> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        var patient = await _context.PatientRepository.GetByIdAsync(request.id);
        if (patient == null)
            throw new NotFoundException("Patient", request.id);

        var isAdministrator = string.Equals(_currentUser.Role, "Administrator", StringComparison.OrdinalIgnoreCase);
        var isOwnPatient = string.Equals(patient.Email, _currentUser.Email, StringComparison.OrdinalIgnoreCase);

        if (!isAdministrator && !isOwnPatient)
        {
            if (string.Equals(_currentUser.Role, "Physician", StringComparison.OrdinalIgnoreCase))
            {
                var physician = await _context.PhysicianRepository.FirstOrDefaultAsync(p => p.Email == _currentUser.Email);
                var appointments = physician == null
                    ? Enumerable.Empty<Appointment>()
                    : await _context.AppointmentRepository.FindAsync(a =>
                        a.PatientId == patient.Id && a.PhysicianId == physician.Id);

                if (!appointments.Any())
                    throw new ForbiddenAccessException();
            }
            else
            {
                throw new ForbiddenAccessException();
            }
        }

        var patientDto = _mapper.Map<PatientDto>(patient);
        return OperationResult.Successful(patientDto);
    }
}
