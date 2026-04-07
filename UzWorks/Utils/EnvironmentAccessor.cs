using UzWorks.Core.Abstract;
using UzWorks.Core.Constants;
using UzWorks.Core.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UzWorks.Identity.Constants;

namespace UzWorks.API.Utils;

public class EnvironmentAccessor(
    IWebHostEnvironment _environment, 
    IHttpContextAccessor _contextAccessor) 
        : IEnvironmentAccessor
{
    public string GetFullName()
    {
        var firstName = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimNames.FirstName.ToString()))?.Value;
        var lastName = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimNames.LastName.ToString()))?.Value;
        return $"{firstName} {lastName}";
    }

    public string GetWebRootPath() =>
        _environment.WebRootPath;

    public bool HasRole(string role) =>
        throw new NotImplementedException();

    public bool IsAdmin(Guid id)
    {
        if (_contextAccessor.HttpContext is null)
            throw new UzWorksException("HttpContext can not be null.");

        if (_contextAccessor.HttpContext.User.IsInRole(RoleNames.SuperAdmin) ||
            _contextAccessor.HttpContext.User.IsInRole(RoleNames.Supervisor))
            return true;

        return false;
    }

    public bool IsAuthorOrAdmin(Guid id)
    {
        if (_contextAccessor.HttpContext is null)
            throw new UzWorksException("HttpContext can not be null.");

        if (_contextAccessor.HttpContext.User.IsInRole(RoleNames.SuperAdmin) || GetUserId() == id.ToString())
            return true;

        return false;
    }

    public bool IsAuthorOrSupervisor(Guid? id)
    {
        if (_contextAccessor.HttpContext is null)
            throw new UzWorksException("HttpContext can not be null.");

        if (_contextAccessor.HttpContext.User.IsInRole(RoleNames.Supervisor) || GetUserId() == id.ToString())
            return true;

        return false;
    }

    public string GetUserId()
    {
        if (_contextAccessor.HttpContext is null)
            throw new UzWorksException("HttpContext can not be null.");

        var claims = _contextAccessor.HttpContext.User.Claims;
        var userId = claims.FirstOrDefault(x => x.Type == ClaimNames.UserId)?.Value
                     ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value
                     ?? claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            throw new UzWorksException("UserId claim not found in token.");

        return userId;
    }

    public string GetUserName() =>
        _contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimNames.UserName.ToString()))?.Value;
}

