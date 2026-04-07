using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UzWorks.API.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[Authorize]
public class BaseController : ControllerBase
{
}
