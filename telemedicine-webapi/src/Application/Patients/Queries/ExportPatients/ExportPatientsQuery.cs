using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace telemedicine_webapi.Application.Patients.Queries.ExportPatients;

public record ExportPatientsQuery : IRequest<ExportPatientVm>
{
    public int ListId { get; init; }
}

public class ExportPatientsQueryHandler : IRequestHandler<ExportPatientsQuery, ExportPatientVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICsvFileBuilder _fileBuilder;

    public ExportPatientsQueryHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportPatientVm> Handle(ExportPatientsQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.Patients
                .Where(t => t.Id == request.ListId)
                .ProjectTo<PatientFileRecord>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

        var vm = new ExportPatientVm(
            "TodoItems.csv",
            "text/csv",
            _fileBuilder.BuildPatientsFile(records));

        return vm;
    }
}
