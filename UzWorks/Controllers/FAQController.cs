using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.FAQs;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.FAQs;

namespace UzWorks.API.Controllers;

public class FAQController(IFAQService _faqService) : BaseController
{
    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost]
    public async Task<ActionResult<FAQVM>> Create([FromBody] FAQDto dto) =>
        Ok(await _faqService.Create(dto));

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FAQVM>>> GetAll() =>
        Ok(await _faqService.GetAllAsync());

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<FAQVM>> GetById(Guid id) =>
        Ok(await _faqService.GetById(id));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost]
    public async Task<ActionResult<FAQVM>> Update([FromBody] FAQEM EM) =>
        Ok(await _faqService.Update(EM));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult<bool>> Delete(Guid id) =>
        Ok(await _faqService.Delete(id));
}
