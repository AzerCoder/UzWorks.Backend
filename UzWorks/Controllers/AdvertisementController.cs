using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Advertisements;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.Advertisements;

namespace UzWorks.API.Controllers;

public class AdvertisementController(IAdvertisementService _advertisementService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = RoleNames.SuperAdmin)]
    public async Task<ActionResult<AdvertisementVM>> Create([FromBody] AdvertisementDto advertisementDto) =>
        Ok(await _advertisementService.CreateAsync(advertisementDto));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdvertisementVM>>> GetAll() =>
        Ok(await _advertisementService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<AdvertisementVM>> GetById([FromRoute] Guid id) =>
        Ok(await _advertisementService.GetByIdAsync(id));
    
    [HttpPost]
    [Authorize(Roles = RoleNames.SuperAdmin)]
    public async Task<ActionResult<AdvertisementVM>> UpdateAdvertisement([FromBody] AdvertisementEM advertisement) =>
        Ok(await _advertisementService.UpdateAsync(advertisement));

    [HttpPost("{id}")]
    [Authorize(Roles = RoleNames.SuperAdmin)]
    public async Task<ActionResult> DeleteAdvertisement([FromRoute] Guid id)
    {
        await _advertisementService.DeleteAsync(id);
        return Ok();
    }
}
