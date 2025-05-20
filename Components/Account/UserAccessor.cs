using Microsoft.AspNetCore.Identity;
using SimpleVetBooking.Data.Models;

namespace SimpleVetBooking.Components.Account;

internal sealed class UserAccessor : IDisposable
{
    private readonly IServiceScope _scope;
    private readonly UserManager<AppUser> _userManager;

    public UserAccessor(IServiceProvider serviceProvider)
    {
        _scope = serviceProvider.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    }

    public Task<AppUser?> GetRequiredUserAsync(string userId)
    {
        return _userManager.FindByIdAsync(userId);
    }

    public void Dispose() => _scope.Dispose();
}

internal sealed class IdentityUserAccessor(UserManager<AppUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<AppUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/Login", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user!;
    }
} 