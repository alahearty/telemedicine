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
            if (_context.Database.IsSqlServer())
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
        // Default roles
        var administratorRole = new IdentityRole<Guid> { Name = "Administrator" };
        var physicianRole = new IdentityRole<Guid> { Name = "Physician" };
        var patientRole = new IdentityRole<Guid> { Name = "Patient" };

        var result = new IdentityResult();

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            result = await _roleManager.CreateAsync(administratorRole);

            var administrator = new IdentityUser<Guid> { UserName = "administrator@localhost", Email = "administrator@localhost"};

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                result = await _userManager.CreateAsync(administrator, "Administrator1!");
                result = await _userManager.AddToRoleAsync(administrator, administratorRole.ToString());
            }
        }

        if (_roleManager.Roles.All(r => r.Name != patientRole.Name))
        {
            result = await _roleManager.CreateAsync(patientRole);
        }
        
        if (_roleManager.Roles.All(r => r.Name != physicianRole.Name))
        {
            result = await _roleManager.CreateAsync(physicianRole);
        }

        var item1 = new TodoItem { Title = "Make a todo list 📃", Id = 1 };
        var item2 = new TodoItem { Title = "Check off the first item ✅", Id = 2 };
        var item3 = new TodoItem { Title = "Realise you've already done two things on the list! 🤯", Id = 3 };
        var item4 = new TodoItem { Title = "Reward yourself with a nice, long nap 🏆", Id = 4 };

        if (!_context.TodoItems.Any())
        {
            _context.TodoItems.Add(item1);
            _context.TodoItems.Add(item2);
            _context.TodoItems.Add(item3);
            _context.TodoItems.Add(item4);
        }

        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items = { item1, item2, item3, item4 }
            });          
        }
        await _context.SaveChangesAsync();
    }
}
