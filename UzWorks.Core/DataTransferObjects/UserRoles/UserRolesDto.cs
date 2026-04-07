using UzWorks.Core.DataTransferObjects.Roles;

namespace UzWorks.Core.DataTransferObjects.UserRoles;

public class UserRolesDto
{
    public Guid UserId { get; set; }
    public IEnumerable<RoleDto>? Roles { get; set; }
}
