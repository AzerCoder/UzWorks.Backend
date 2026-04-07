using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Workers;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.Workers;

namespace UzWorks.API.Controllers;

public class WorkerController(IWorkerService _workerService) : BaseController
{
    [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.SuperAdmin},{RoleNames.Supervisor}")]
    [HttpPost]
    public async Task<ActionResult<WorkerVM>> Create([FromBody] WorkerDto workerDto) =>
        Ok(await _workerService.Create(workerDto));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkerVM>>> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize,
                                            [FromQuery] Guid? jobCategoryId, [FromQuery] int? maxAge,
                                            [FromQuery] int? minAge, [FromQuery] uint? maxSalary,
                                            [FromQuery] uint? minSalary, [FromQuery] int? gender,
                                            [FromQuery] Guid? regionId, [FromQuery] Guid? districtId) =>
        Ok(await _workerService.GetAllAsync(
                         pageNumber, pageSize, jobCategoryId,
                         maxAge, minAge, maxSalary, minSalary,
                         gender, true, regionId, districtId));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkerVM>> GetById([FromRoute] Guid id) =>
        Ok(await _workerService.GetById(id));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkerVM>>> GetTopWorkers() =>
        Ok(await _workerService.GetTops());

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkerVM>>> GetAllForAdmin([FromQuery] int pageNumber, [FromQuery] int pageSize,
                                            [FromQuery] Guid? jobCategoryId, [FromQuery] int? maxAge,
                                            [FromQuery] int? minAge, [FromQuery] uint? maxSalary,
                                            [FromQuery] uint? minSalary, [FromQuery] int? gender,
                                            [FromQuery] Guid? regionId, [FromQuery] Guid? districtId) =>
        Ok(await _workerService.GetAllAsync(
                         pageNumber, pageSize, jobCategoryId,
                         maxAge, minAge, maxSalary, minSalary,
                         gender, null, regionId, districtId));

    [AllowAnonymous]
    [HttpGet("{status}")]
    public async Task<ActionResult<int>> GetCount([FromRoute]bool? status) =>
        Ok(await _workerService.GetCount(status));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<int>> GetCountForFilter(
                                            [FromQuery] Guid? jobCategoryId, [FromQuery] int? maxAge,
                                            [FromQuery] int? minAge, [FromQuery] uint? maxSalary,
                                            [FromQuery] uint? minSalary, [FromQuery] int? gender,
                                            [FromQuery] Guid? regionId, [FromQuery] Guid? districtId) =>
        Ok(await _workerService.GetCountForFilter(jobCategoryId,
                         maxAge, minAge, maxSalary, minSalary,
                         gender, true, regionId, districtId));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<WorkerVM>>> GetWorkersByUserId([FromRoute] Guid id) =>
        Ok(await _workerService.GetByUserId(id));

    [Authorize(Roles = RoleNames.Employee)]
    [HttpPost]
    public async Task<ActionResult<WorkerVM>> Update([FromBody] WorkerEM workerEM) =>
        Ok(await _workerService.Update(workerEM));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Activate([FromRoute] Guid id)
    {
        await _workerService.ChangeStatus(id, true);
        return Ok(await _workerService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Deactivate([FromRoute] Guid id)
    {
        await _workerService.ChangeStatus(id, false);
        return Ok(await _workerService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> MakeATop([FromRoute] Guid id)
    {
        await _workerService.ChangeTop(id, true);
        return Ok(await _workerService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult> StopTheTop([FromRoute] Guid id)
    {
        await _workerService.ChangeTop(id, false);
        return Ok(await _workerService.GetById(id));
    }

    [Authorize(Roles = RoleNames.Employee)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _workerService.Delete(id);
        return Ok();
    }
}

