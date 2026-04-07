using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Workers.Experiences;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.Experiences;

namespace UzWorks.API.Controllers;

public class ExperienceController(IExperienceService _experienceService) : BaseController
{
    [Authorize(Roles = RoleNames.Employee)]
    [HttpPost]
    public async Task<ActionResult<ExperienceVM>> Create([FromBody] ExperienceDto experienceDto) =>
        Ok(await _experienceService.Create(experienceDto));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExperienceVM>>> GetAll() =>
        Ok(await _experienceService.GetAll());

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ExperienceVM>> GetById([FromRoute] Guid id) =>
        Ok(await _experienceService.GetById(id));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<ExperienceVM>>> GetByUserId([FromRoute] Guid id) =>
        Ok(await _experienceService.GetByUserId(id));

    [Authorize(Roles = RoleNames.Employee)]
    [HttpPost]
    public async Task<ActionResult<ExperienceVM>> Update([FromBody] ExperienceEM experienceEM) =>
        Ok(await _experienceService.Update(experienceEM));

    [Authorize(Roles = RoleNames.Employee)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id) =>
        await _experienceService.Delete(id) ? Ok() : BadRequest("Did not delete.");
}
