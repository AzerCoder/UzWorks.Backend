using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.JobCategories;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.JobCategories;

namespace UzWorks.API.Controllers;

public class JobCategoryController(IJobCategoryService _service) : BaseController
{
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost]
    public async Task<ActionResult<JobCategoryVM>> Create(JobCategoryDto jobCategoryDto) =>
        Ok(await _service.Create(jobCategoryDto));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobCategoryVM>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<JobCategoryVM>> GetById([FromRoute] Guid id) =>
        Ok(await _service.GetById(id));

    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost]
    public async Task<ActionResult<JobCategoryVM>> Update([FromBody] JobCategoryEM jobCategoryEM) =>
        Ok(await _service.Update(jobCategoryEM));

    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id) =>
        Ok(await _service.Delete(id));
}
