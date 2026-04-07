using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.FeedBacks;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.FeedBacks;

namespace UzWorks.API.Controllers;

public class FeedBackController(IFeedBackService _feedBackService) : BaseController
{
    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost]
    public async Task<ActionResult<FeedBackVM>> Create([FromBody] FeedBackDto dto) =>
        Ok(await _feedBackService.Create(dto));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeedBackVM>>> GetAll() =>
        Ok(await _feedBackService.GetAllAsync());

    [Authorize]
    [HttpGet("{Id}")]
    public async Task<ActionResult<FeedBackVM>> GetById(Guid Id) =>
        Ok(await _feedBackService.GetById(Id));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost]
    public async Task<ActionResult<FeedBackVM>> Update([FromBody] FeedBackEM EM) =>
        Ok(await _feedBackService.Update(EM));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{Id}")]
    public async Task<ActionResult<bool>> Delete(Guid Id) =>
        Ok(await _feedBackService.Delete(Id));
}