namespace Budget.Spendings.Api.Services;

public class HttpContextUserInspector : IUserInspector
{
    private readonly HttpContext _context;

    public HttpContextUserInspector(IHttpContextAccessor contextAccessor)
    {
        if (contextAccessor.HttpContext == null)
            throw new Exception("Cannot initialize http context user inspector");

        _context = contextAccessor.HttpContext;
    }

    public string GetAuthenticatedUser()
    {
        var value = _context.User.Identity?.Name ?? string.Empty;

        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("No user id available");

        return value;
    }
}