using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Locations.Districts;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.Location.Districts;

namespace UzWorks.API.Controllers;

public class DistrictController(IDistrictService _districtService) : BaseController
{
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost] 
    public async Task<ActionResult<DistrictVM>> Create([FromBody]DistrictDto district) =>
        Ok(await _districtService.Create(district));

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<DistrictVM>> GetById([FromRoute]Guid id) =>
        Ok(await _districtService.GetById(id));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DistrictVM>>> GetAll() =>
        Ok(await _districtService.GetAllAsync());

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<DistrictVM>>> GetByRegionId([FromRoute]Guid id) =>
        Ok(await _districtService.GetByRegionId(id));
    
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost]
    public async Task<ActionResult<DistrictVM>> Update([FromBody]DistrictEM districtEM) =>
        Ok(await _districtService.Update(districtEM));

    
    [Authorize(Roles = RoleNames.SuperAdmin)]
    [HttpPost("{id}")]
    public async Task<ActionResult> Delete([FromRoute]Guid id) =>
        await _districtService.Delete(id) ? Ok() : BadRequest();
}
