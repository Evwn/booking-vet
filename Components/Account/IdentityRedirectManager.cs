using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace SimpleVetBooking.Components.Account;

internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    public void RedirectTo(string? uri)
        => navigationManager.NavigateTo(uri ?? "");

    public void RedirectTo(string? uri, Dictionary<string, object?> queryParameters)
        => navigationManager.NavigateTo(navigationManager.GetUriWithQueryParameters(uri ?? "", queryParameters));

    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append("Identity.StatusMessage", message);
        navigationManager.NavigateTo(uri);
    }

    public void RedirectToCurrentPage()
        => navigationManager.Refresh(forceReload: true);
} 