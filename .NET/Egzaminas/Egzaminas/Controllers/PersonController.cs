using System.Security.Claims;
using Egzaminas.Models.DTOs;
using Egzaminas.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Egzaminas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User ID not found in token");
            }
            return userId;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        [HttpPost("AddPersonInfo")]
        public async Task<IActionResult> AddPersonInfo([FromForm] PersonInfoDto personDto)
        {
            try
            {
                var userId = GetUserId();
                var person = await _personService.AddPersonInfo(userId, personDto);
                return Ok(new { message = "Personal information added successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        [HttpGet("GetPersonInfo")]
        public async Task<IActionResult> GetPersonInfo()
        {
            try
            {
                var userId = GetUserId();
                var person = await _personService.GetPersonInfo(userId);
                return Ok(person);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        [HttpPatch("UpdatePersonInfo")]
        public async Task<IActionResult> UpdatePersonInfo([FromForm] UpdatePersonInfoDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _personService.UpdatePersonInfo(userId, dto);
                return Ok(new { message = "Personal information updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="User")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangeUserPassword(string newPassword)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim.Value);

            var (success, message) = await _personService.ChangeUserPasswordAsync(userId, newPassword);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("name")]
        // public async Task<IActionResult> UpdateName([FromForm] string name)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdateName(userId, name);
        //         return Ok(new { message = "Name updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("surname")]
        // public async Task<IActionResult> UpdateSurname([FromForm] string surname)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdateSurname(userId, surname);
        //         return Ok(new { message = "Surname updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("personalNumber")]
        // public async Task<IActionResult> UpdatePersonalNumber([FromForm] string personalNumber)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdatePersonalNumber(userId, personalNumber);
        //         return Ok(new { message = "Personal number updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("phoneNumber")]
        // public async Task<IActionResult> UpdatePhoneNumber([FromForm] string phoneNumber)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdatePhoneNumber(userId, phoneNumber);
        //         return Ok(new { message = "Phone number updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("email")]
        // public async Task<IActionResult> UpdateEmail([FromForm] string email)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdateEmail(userId, email);
        //         return Ok(new { message = "Email updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("address/city")]
        // public async Task<IActionResult> UpdateCity([FromForm] string city)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdateCity(userId, city);
        //         return Ok(new { message = "City updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("address/street")]
        // public async Task<IActionResult> UpdateStreet([FromForm] string street)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdateStreet(userId, street);
        //         return Ok(new { message = "Street updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("address/houseNumber")]
        // public async Task<IActionResult> UpdateHouseNumber([FromForm] int houseNumber)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdateHouseNumber(userId, houseNumber);
        //         return Ok(new { message = "House number updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Admin")]
        // [HttpPut("address/flat-number")]
        // public async Task<IActionResult> UpdateFlatNumber([FromForm] int flatNumber)
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         await _personService.UpdateFlatNumber(userId, flatNumber);
        //         return Ok(new { message = "Flat number updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }
    }
}