using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Contacts;
using UzWorks.Core.Constants;
using UzWorks.Core.DataTransferObjects.Contacts;

namespace UzWorks.API.Controllers;

public class ContactController(IContactService _contactService) : BaseController
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<ContactVM>> Create([FromBody] ContactDto contactDto) =>
        Ok(await _contactService.Create(contactDto));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpGet]
    public async Task<ActionResult<ContactVM>> GetAll(int pageNumber = 1, int pageSize = 15, bool? isComplated = null) =>
        Ok(await _contactService.GetAllAsync(pageNumber, pageSize, isComplated));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ContactVM>> GetById([FromRoute]Guid id) =>
        Ok(await _contactService.GetById(id));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost]
    public async Task<ActionResult<ContactVM>> Update([FromBody] ContactEM contactEM) =>
        Ok(await _contactService.Update(contactEM));

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}/{status}")]
    public async Task<ActionResult<ContactVM>> ChangeStatus(Guid id, bool status) =>
        await _contactService.ChangeStatus(id, status) ? Ok(_contactService.GetById(id)) : BadRequest();

    [Authorize(Roles = RoleNames.Supervisor)]
    [HttpPost("{id}")]
    public async Task<ActionResult<string>> Delete(Guid id) =>
        await _contactService.Delete(id) ? Ok("Delete saccessfull.") : BadRequest();
}
