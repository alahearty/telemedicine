using telemedicine_webapi.Domain.Entities;
using telemedicine_webapi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace telemedicine_webapi.Infrastructure.Persistence.Context;

public class ApplicationDbContextInitialiser:IApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<IdentityUser<Guid>> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        await SeedIdentityAsync();
        await SeedTodoDataAsync();
    }

    private async Task SeedIdentityAsync()
    {
        // Create roles
        var roles = new[] { "Administrator", "Physician", "Patient" };
        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            }
        }

        // Create admin user (stored in AspNetUsers, not the User table)
        const string adminUserName = "administrator@localhost";
        const string adminPassword = "Administrator1!";

        if (await _userManager.FindByNameAsync(adminUserName) == null)
        {
            var administrator = new IdentityUser<Guid>
            {
                UserName = adminUserName,
                Email = adminUserName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(administrator, adminPassword);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(administrator, "Administrator");
                _logger.LogInformation("Admin user created: {UserName}", adminUserName);
            }
            else
            {
                _logger.LogWarning("Failed to create admin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task SeedTodoDataAsync()
    {
        try
        {
            if (await _context.TodoLists.AnyAsync())
                return;

            var todoList = new TodoList { Title = "Todo List" };
            _context.TodoLists.Add(todoList);
            await _context.SaveChangesAsync();

            var items = new[]
            {
                new TodoItem { Title = "Make a todo list 📃", ListId = todoList.Id },
                new TodoItem { Title = "Check off the first item ✅", ListId = todoList.Id },
                new TodoItem { Title = "Realise you've already done two things on the list! 🤯", ListId = todoList.Id },
                new TodoItem { Title = "Reward yourself with a nice, long nap 🏆", ListId = todoList.Id }
            };
            _context.TodoItems.AddRange(items);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Todo data seeding failed (non-critical). Admin user may still have been created.");
        }
    }
}
