using System.Reflection;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace telemedicine_webapi.Infrastructure.Persistence.Context;

public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>, IApplicationDbContext
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
    public DbSet<PhysicianPatientTransaction> PhysianPatientTransactions => Set<PhysicianPatientTransaction>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Entity<Message>()
                .HasOne(x => x.AppUser)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.SenderUserId);
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
