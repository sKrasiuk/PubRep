using Egzaminas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Egzaminas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpDelete("DeleteUserById")]
        public async Task<IActionResult> DeleteUserById(int userId)
        {
            var (success, message) = await _adminService.DeleteUserByIdAsync(userId);

            if (!success)
            {
                return NotFound(new { message });
            }

            return Ok(new { message });
        }

        [HttpDelete("DeleteUserByPersonalNumber")]
        public async Task<IActionResult> DeleteUserByPersonalNumber(string personalNumber)
        {
            var (success, message) = await _adminService.DeleteUserByPersonalNumberAsync(personalNumber);

            if (!success)
            {
                return NotFound(new { message });
            }

            return Ok(new { message });
        }

        [HttpPost("SetUserRole")]
        public async Task<IActionResult> SetUserRole(int userId, string role)
        {
            var (success, message) = await _adminService.SetUserRoleAsync(userId, role);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpPost("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(int userId, string newPassword)
        {
            var (success, message) = await _adminService.ChangeUserPasswordAsync(userId, newPassword);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}
