
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IApplicationDbContext _context;
    public UnitOfWork(IApplicationDbContext context)
    {
        _context = context;
        AppointmentRepository = new GenericRepository<Appointment>(context);
        CommentRepository = new GenericRepository<Comment>(context);
        HealthAnalysisReportRepository = new GenericRepository<HealthAnalysisReport>(context);
        HospitalRepository = new GenericRepository<Hospital>(context);
        PatientRepository = new GenericRepository<Patient>(context);
        PhysicianRepository = new GenericRepository<Physician>(context);
        PhysicianPatientTransactionRepository = new GenericRepository<PhysicianPatientTransaction>(context);
        ScheduleTimeRepository = new GenericRepository<ScheduleTime>(context);
        TelemedicinePaymentRepository = new GenericRepository<TelemedicinePayment>(context);
        TelemedicineServiceRepository = new GenericRepository<TelemedicineService>(context);
        TodoItemRepository = new GenericRepository<TodoItem>(context);
        TodoListRepository = new GenericRepository<TodoList>(context);
        UserRepository = new GenericRepository<User>(context);
    }

    public IGenericRepository<Appointment> AppointmentRepository { get; }
    public IGenericRepository<Comment> CommentRepository { get; }
    public IGenericRepository<HealthAnalysisReport> HealthAnalysisReportRepository { get; }
    public IGenericRepository<Hospital> HospitalRepository { get; }
    public IGenericRepository<Patient> PatientRepository { get; }
    public IGenericRepository<Physician> PhysicianRepository { get; }
    public IGenericRepository<PhysicianPatientTransaction> PhysicianPatientTransactionRepository { get; }
    public IGenericRepository<ScheduleTime> ScheduleTimeRepository { get; }
    public IGenericRepository<TelemedicinePayment> TelemedicinePaymentRepository { get; }
    public IGenericRepository<TelemedicineService> TelemedicineServiceRepository { get; }
    public IGenericRepository<TodoItem> TodoItemRepository { get; }
    public IGenericRepository<TodoList> TodoListRepository { get; }
    public IGenericRepository<User> UserRepository { get; }

    public async Task<BaseResponse> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);
        if (result > 0) return OperationResult.Successful();
        return OperationResult.NotSuccessful("unable to save changes");
    }
}
