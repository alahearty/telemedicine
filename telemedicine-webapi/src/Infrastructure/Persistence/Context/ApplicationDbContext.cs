using System.Reflection;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Infrastructure.Identity;
using telemedicine_webapi.Infrastructure.Persistence.Interceptors;
using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace telemedicine_webapi.Infrastructure.Persistence.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, UserRole, int>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
        /*IOptions<OperationalStoreOptions> operationalStoreOptions,*/)
        : base(options/*, operationalStoreOptions*/)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Physician> Physicians => Set<Physician>();
    public DbSet<TelemedicineService> TelemedicineServices => Set<TelemedicineService>();
    public DbSet<TelemedicinePayment> TelemedicinePayments => Set<TelemedicinePayment>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ScheduleTime> ScheduleTimes => Set<ScheduleTime>();
    public DbSet<PhysicianPatientTransaction> PhysianPatientTransactions => Set<PhysicianPatientTransaction>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
}
