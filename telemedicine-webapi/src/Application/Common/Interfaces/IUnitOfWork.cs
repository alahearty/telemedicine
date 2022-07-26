using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Domain.Entities;

namespace telemedicine_webapi.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Appointment> AppointmentRepository { get; }
    IGenericRepository<Comment> CommentRepository { get; }
    IGenericRepository<HealthAnalysisReport> HealthAnalysisReportRepository { get; }
    IGenericRepository<Hospital> HospitalRepository { get; }
    IGenericRepository<Patient> PatientRepository { get; }
    IGenericRepository<Physician> PhysicianRepository { get; }
    IGenericRepository<PhysicianPatientTransaction> PhysicianPatientTransactionRepository { get; }
    IGenericRepository<TelemedicinePayment> TelemedicinePaymentRepository { get; }
    IGenericRepository<TelemedicineService> TelemedicineServiceRepository { get; }
    IGenericRepository<TodoItem> TodoItemRepository { get; }
    IGenericRepository<TodoList> TodoListRepository { get; }
    IGenericRepository<User> UserRepository { get; }
    Task<BaseResponse> SaveChangesAsync(CancellationToken cancellationToken = default);
}
