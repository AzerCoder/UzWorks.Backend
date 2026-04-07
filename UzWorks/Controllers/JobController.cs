using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Jobs;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.Jobs;

namespace UzWorks.API.Controllers;

public class JobController(IJobService _jobService) : BaseController
{
    [Authorize(Roles = $"{RoleNames.Employer},{RoleNames.SuperAdmin},{RoleNames.Supervisor}")]
    [HttpPost]
    public async Task<ActionResult<JobVM>> Create(JobDto jobDto) =>
        Ok(await _jobService.Create(jobDto));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobVM>>> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize,
                                            [FromQuery] Guid? jobCategoryId, [FromQuery] int? maxAge,
                                            [FromQuery] int? minAge, [FromQuery] uint? maxSalary,
                                            [FromQuery] uint? minSalary, [FromQuery] int? gender,
                                            [FromQuery] Guid? regionId, [FromQuery] Guid? districtId) =>
        Ok(await _jobService.GetAllAsync(
                         pageNumber, pageSize, jobCategoryId,
                         maxAge, minAge, maxSalary, minSalary,
                         gender, true, regionId, districtId));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<JobVM>> GetById([FromRoute] Guid id) =>
        Ok(await _jobService.GetById(id));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobVM>>> GetTopJobs() =>
        Ok(await _jobService.GetTops());

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobVM>>> GetAllForAdmin([FromQuery] int pageNumber, [FromQuery] int pageSize,
                                            [FromQuery] Guid? jobCategoryId, [FromQuery] int? maxAge,
                                            [FromQuery] int? minAge, [FromQuery] uint? maxSalary,
                                            [FromQuery] uint? minSalary, [FromQuery] int? gender,
                                            [FromQuery] Guid? regionId, [FromQuery] Guid? districtId) =>

        Ok(await _jobService.GetAllAsync(
                         pageNumber, pageSize, jobCategoryId,
                         maxAge, minAge, maxSalary, minSalary,
                         gender, null, regionId, districtId));

    [AllowAnonymous]
    [HttpGet("{status}")]
    public async Task<ActionResult<int>> GetCount([FromRoute]bool? status) =>
        Ok(await _jobService.GetCount(status));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<int>> GetCountForFilter(
                                            [FromQuery] Guid? jobCategoryId, [FromQuery] int? maxAge,
                                            [FromQuery] int? minAge, [FromQuery] uint? maxSalary,
                                            [FromQuery] uint? minSalary, [FromQuery] int? gender,
                                            [FromQuery] Guid? regionId, [FromQuery] Guid? districtId) =>

        Ok(await _jobService.GetCountForFilter(jobCategoryId,
                         maxAge, minAge, maxSalary, minSalary,
                         gender, true, regionId, districtId));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<JobVM>>> GetByUserId([FromRoute] Guid id) =>
        Ok(await _jobService.GetByUserId(id));

    [Authorize(Roles = RoleNames.Employer)]
    [HttpPost]
    public async Task<ActionResult<JobVM>> Update([FromBody] JobEM jobEM) =>
        Ok(await _jobService.Update(jobEM));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Activate([FromRoute] Guid id)
    {
        await _jobService.ChangeStatus(id, true);

        return Ok(await _jobService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Deactivate([FromRoute] Guid id) 
    { 
        await _jobService.ChangeStatus(id, false);

        return Ok(await _jobService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> MakeATop([FromRoute] Guid id) 
    {
        await _jobService.ChangeTop(id, true);
        return Ok(await _jobService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> StopTheTop([FromRoute] Guid id) 
    {
        await _jobService.ChangeTop(id, false);
        return Ok(await _jobService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Employer)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id) =>
        (await _jobService.Delete(id)) ? Ok() : BadRequest();
}
