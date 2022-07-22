using AutoMapper;
using AutoMapper.QueryableExtensions;
using telemedicine_webapi.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Hospitals.Queries.ExportHospitals;

public record ExportHospitalsQuery : IRequest<ExportHospitalsVm>
{
    public int ListId { get; init; }
}

public class ExportTodosQueryHandler : IRequestHandler<ExportHospitalsQuery, ExportHospitalsVm>
{
    private readonly IUnitOfWork _context;
    private readonly IMapper _mapper;
    private readonly ICsvFileBuilder _fileBuilder;

    public ExportTodosQueryHandler(IUnitOfWork context, IMapper mapper, ICsvFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task<ExportHospitalsVm> Handle(ExportHospitalsQuery request, CancellationToken cancellationToken)
    {
        var hospitals=await _context.HospitalRepository.GetAllAsync();
        var records = await hospitals.AsQueryable<Hospital>()
                .ProjectTo<HospitalRecord>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

        var vm = new ExportHospitalsVm(
            "TodoItems.csv",
            "text/csv",
            _fileBuilder.BuildHostpitalsFile(records));

        return vm;
    }
}
