using Microsoft.AspNetCore.Mvc;
using UzWorks.Core.DataTransferObjects.Auth;
using UzWorks.Identity.Services.Auth;

namespace UzWorks.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync([FromBody] LoginDto loginDto) =>
        Ok(await _authService.Login(loginDto));

    [HttpPost]
    [Route("signup")]
    public async Task<ActionResult<SignUpResponseDto>> SignUpAsync([FromBody] SignUpDto signUpDto) =>
        Ok(await _authService.Register(signUpDto));

    [HttpPost]
    [Route("verify-phone")]
    public async Task<IActionResult> VerifyPhoneNumber([FromBody] VerifyPhoneNumberDto verifyDto) =>
        (await _authService.VerifyPhoneNumber(verifyDto.PhoneNumber, verifyDto.Code))?
            Ok("Successfull!"):
            BadRequest("Not Saccessfull!");

    [HttpPost]
    [Route("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto) =>
        (await _authService.ForgetPassword(forgetPasswordDto.PhoneNumber))?
            Ok("Successfull!"):
            BadRequest("Not Saccessfull!");

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordByCodeDto resetPasswordDto) =>
        (await _authService.ResetPassword(resetPasswordDto.PhoneNumber, resetPasswordDto.NewPassword, resetPasswordDto.Code))?
            Ok("Successfull!"):
            BadRequest("Not Saccessfull!");
}
