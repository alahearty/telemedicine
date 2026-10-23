using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using telemedicine_webapi.Application.Common.Interfaces;
using telemedicine_webapi.Application.Common.Models;
using telemedicine_webapi.Application.Common.Models.Authentication;
using telemedicine_webapi.Application.Services;
using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Domain.Enums;
using telemedicine_webapi.Infrastructure.Persistence.Context;

namespace telemedicine_webapi.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public AuthService(IJwtTokenGenerator jwtTokenGenerator, IIdentityService identityService, IApplicationDbContext context)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _identityService = identityService;
        _context = context;
    }

    public async Task<BaseResponse> Register(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        var role = NormalizeAccountType(request.AccountType);
        if (role is null)
            return OperationResult.NotSuccessful("Account type must be Patient or Physician");
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            return OperationResult.NotSuccessful("Email and password are required");

        var firstName = request.FirstName?.Trim();
        var lastName = request.LastName?.Trim();
        var phone = request.Phone?.Trim();
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(phone))
            return OperationResult.NotSuccessful("First name, last name and phone are required");
        if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
            return OperationResult.NotSuccessful("Gender must be Male, Female or Other");
        if (request.Birth is null || request.Birth.Value == default)
            return OperationResult.NotSuccessful("Birth date is required");
        if (request.Birth.Value.Date > DateTime.UtcNow.Date)
            return OperationResult.NotSuccessful("Birth date cannot be in the future");

        var license = request.License?.Trim();
        var specialization = request.MedicalSpecialization?.Trim();
        if (role == "Physician")
        {
            if (string.IsNullOrWhiteSpace(license) || string.IsNullOrWhiteSpace(specialization))
                return OperationResult.NotSuccessful("Licence number and medical speciality are required for physicians");
        }

        var existingAccount = await _identityService.GetUserByEmailAsync(email);
        if (existingAccount.Data != null)
            return OperationResult.NotSuccessful($"Account with email-{email} already exists");

        var existingProfile = role == "Patient"
            ? await _context.Patients.AnyAsync(x => x.Email == email, cancellationToken)
            : await _context.Physicians.AnyAsync(x => x.Email == email || x.License == license, cancellationToken);
        if (existingProfile)
            return OperationResult.NotSuccessful("A profile already exists for these details");

        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        try
        {
            var createUserOperation = await _identityService.CreateUserAsync(email, request.Password, role);
            if (createUserOperation.NotSuccesful)
            {
                if (transaction is not null) await transaction.RollbackAsync(cancellationToken);
                return OperationResult.NotSuccessful(createUserOperation.Errors);
            }

            var identityUser = createUserOperation.Data as IdentityUser<Guid>;
            if (identityUser is null)
                return OperationResult.NotSuccessful("Unable to create the identity account");

            int profileId;
            if (role == "Patient")
            {
                var patient = new Patient
                {
                    IdentityUserId = identityUser.Id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Gender = gender,
                    Birth = request.Birth.Value.Date,
                    Phone = phone,
                    Address = request.Address?.Trim(),
                    EmergencyContactName = request.EmergencyContactName?.Trim(),
                    EmergencyContactPhone = request.EmergencyContactPhone?.Trim(),
                    AccountType = AccountType.PATIENT,
                    Created = DateTime.UtcNow,
                    CreatedBy = email
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync(cancellationToken);
                profileId = patient.Id;
            }
            else
            {
                var physician = new Physician
                {
                    IdentityUserId = identityUser.Id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Gender = gender,
                    Birth = request.Birth.Value.Date,
                    Phone = phone,
                    Address = request.Address?.Trim(),
                    License = license,
                    MedicalSpecialization = specialization,
                    Qualifications = request.Qualifications?.Trim(),
                    Bio = request.Bio?.Trim(),
                    AccountType = AccountType.PHYSICIAN,
                    Created = DateTime.UtcNow,
                    CreatedBy = email
                };
                _context.Physicians.Add(physician);
                await _context.SaveChangesAsync(cancellationToken);
                profileId = physician.Id;
            }

            if (transaction is not null) await transaction.CommitAsync(cancellationToken);

            return OperationResult.Successful(new { id = profileId, role, email });
        }
        catch (DbUpdateException ex) when (IsConstraintViolation(ex))
        {
            if (transaction is not null) await transaction.RollbackAsync(cancellationToken);
            return OperationResult.NotSuccessful("An account or profile with these details already exists");
        }
        catch
        {
            if (transaction is not null) await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<BaseResponse> Login(string email, string password)
    {
        var fetchedUser = await _identityService.GetUserByEmailAsync(email);
        if (fetchedUser.NotSuccesful) return OperationResult.NotSuccessful($"Account with email-{email} does not exist");

        var user = fetchedUser.Data as IdentityUser<Guid>;

        if (user?.PasswordHash is null) return OperationResult.NotSuccessful("Provided password is incorrect");
        var verifyPassword = new PasswordHasher<IdentityUser<Guid>>().VerifyHashedPassword(user, user.PasswordHash, password);
        if (verifyPassword == PasswordVerificationResult.Failed) return OperationResult.NotSuccessful("Provided password is incorrect");

        var role = await _identityService.GetUserRoleAsync(user.Email!);
        if (string.IsNullOrWhiteSpace(role)) return OperationResult.NotSuccessful("Account has no assigned role");

        var token = _jwtTokenGenerator.GenerateJwtToken(user.Id, user.Email!, role);
        return OperationResult.Successful(new { token, role });
    }

    private static string? NormalizeAccountType(string? accountType)
    {
        return accountType?.Trim().ToLowerInvariant() switch
        {
            "patient" => "Patient",
            "physician" or "doctor" => "Physician",
            _ => null
        };
    }

    private static bool IsConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: "23505" or "23P01" }
            || exception.InnerException?.InnerException is PostgresException { SqlState: "23505" or "23P01" };
    }
}
