using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Patients.Queries.ExportPatients;

public record ExportPatientsQuery : IRequest<ExportPatientVm>
{
    public int ListId { get; init; }
}

public class ExportPatientsQueryHandler : IRequestHandler<ExportPatientsQuery, ExportPatientVm>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;
    private readonly ICsvFileBuilder _fileBuilder;

    public ExportPatientsQueryHandler(IUnitOfWork context, IMapper mapper, ICsvFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportPatientVm> Handle(ExportPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _context.PatientRepository.FindAsync(t => t.Id == request.ListId);
        var records=await patients.AsQueryable<Patient>().ProjectTo<PatientFileRecord>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

        var vm = new ExportPatientVm(
            "TodoItems.csv",
            "text/csv",
            _fileBuilder.BuildPatientsFile(records));

        return vm;
    }
}
