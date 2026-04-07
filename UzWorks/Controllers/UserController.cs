using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.UserRoles;
using UzWorks.Core.DataTransferObjects.Users;
using UzWorks.Core.Enums.GenderTypes;
using UzWorks.Identity.Services.Roles;

namespace UzWorks.API.Controllers;

public class UserController(IUserService _userService) : BaseController
{
    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] UserDto user)
    {
        await _userService.Create(user);
        return Ok();
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery]int pageNumber, [FromQuery]int pageSize, 
        [FromQuery]GenderEnum? gender, [FromQuery] string? email, 
        [FromQuery] string? phoneNumber) =>
        Ok(await _userService.GetAll(pageNumber, pageSize, gender, email, phoneNumber));

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserVM>> GetById([FromRoute]Guid id) =>
        Ok(await _userService.GetById(id));

    [Authorize]
    [HttpGet("{id}")]   
    public async Task <ActionResult<IEnumerable<string>>> GetRoles([FromRoute] Guid id) =>
        Ok(await _userService.GetUserRoles(id));

    [Authorize]
    [HttpGet]
    public async Task <ActionResult<int>> GetCount() =>
        Ok(await _userService.GetCount());

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<UserVM>> Update([FromBody] UserEM profileEM) =>
        Ok(await _userService.Update(profileEM));

    [HttpPost]
    [Authorize] 
    public async Task<ActionResult<ResetPasswordDto>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto) =>
        await _userService.ResetPassword(resetPasswordDto) ? Ok() : BadRequest();

    [HttpPost("{userId}")]
    [Authorize(Roles = RoleNames.SuperAdmin)]
    public async Task<ActionResult> ResetPasswordForAdmin([FromRoute]Guid userId, [FromBody] string newPassword) =>
        await _userService.ResetPassword(userId, newPassword) ? Ok() : BadRequest();

    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost]
    public async Task<ActionResult> AddRolesToUser([FromBody] UserRolesDto userRoles)
    {
        await _userService.AddRolesToUser(userRoles);
        return Ok();
    }

    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost]
    public async Task<ActionResult> DeleteRolesFromUser([FromBody] UserRolesDto userRoles) 
    {
        await _userService.DeleteRolesFromUser(userRoles);
        return Ok();
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Delete([FromRoute]Guid id) =>
        await _userService.Delete(id) ? Ok() : BadRequest();
}
