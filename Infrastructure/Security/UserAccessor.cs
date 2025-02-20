using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Security;

public class UserAccessor : IUserAccessor
{
    private readonly ILogger<UserAccessor> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor, ILogger<UserAccessor> logger)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetUsername()
    {
        _logger.LogInformation("Attempting to retrieve the username from HttpContext...");
        Console.WriteLine("Attempting to retrieve the username from HttpContext from user accessor...");

        var username = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("No username found in HttpContext.");
            Console.WriteLine("No username found in HttpContext.");
        }
        else
        {
            _logger.LogInformation($"Successfully retrieved username: {username}");
            Console.WriteLine($"Successfully retrieved username: {username}");
        }

        return username ?? string.Empty;
    }

        
    public string GetUserId()
    {
        _logger.LogInformation("Attempting to retrieve the User Id from HttpContext...");
        Console.WriteLine("Attempting to retrieve the User Id from HttpContext from user accessor...");

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("No user id found in HttpContext.");
            Console.WriteLine("No user id found in HttpContext.");
        }
        else
        {
            _logger.LogInformation($"Successfully retrieved user id: {userId}");
            Console.WriteLine($"Successfully retrieved user id: {userId}");
        }

        return userId ?? string.Empty;
    }
}