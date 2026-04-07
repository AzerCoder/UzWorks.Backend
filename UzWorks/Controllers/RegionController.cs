using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Locations.Regions;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.Location.Regions;

namespace UzWorks.API.Controllers;

public class RegionController(IRegionsService _regionsService) : BaseController
{
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost]
    public async Task<ActionResult<RegionVM>> Create(RegionDto regionDto) =>
        Ok(await _regionsService.Create(regionDto));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionVM>>> GetAll() =>
        Ok(await _regionsService.GetAllAsync());

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<RegionVM>> GetById([FromRoute]Guid id) =>
        Ok(await _regionsService.GetById(id));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<RegionVM>> GetByDistrictId([FromRoute]Guid id) =>
        Ok(await _regionsService.GetByDistrictId(id));

    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost]
    public async Task<ActionResult<RegionVM>> Update([FromBody]RegionEM regionEM) =>
        Ok(await _regionsService.Update(regionEM));

    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id) =>
        await _regionsService.Delete(id) ? Ok() : BadRequest();
}
