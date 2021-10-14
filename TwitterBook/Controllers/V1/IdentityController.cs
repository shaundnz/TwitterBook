using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.DTO.V1.Requests;
using TwitterBook.DTO.V1.Responses;
using TwitterBook.Services;

namespace TwitterBook.Controllers.V1
{
    [ApiController]
    [Route("/api/identity")]
    public class IdentityController : Controller
    {

        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDTO userRegistration)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponseDTO { Errors = ModelState.Values.SelectMany(val => val.Errors.Select(err => err.ErrorMessage)) });
            }

            var authResponse = await _identityService.RegisterAsync(userRegistration.Email, userRegistration.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponseDTO
                {
                    Errors = authResponse.Errors
                });

            }

            return Ok(new AuthSuccessResponseDTO { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO userRegistration)
        {

            var authResponse = await _identityService.LoginAsync(userRegistration.Email, userRegistration.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponseDTO
                {
                    Errors = authResponse.Errors
                });

            }

            return Ok(new AuthSuccessResponseDTO { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO userRegistration)
        {

            var authResponse = await _identityService.RefreshTokenAsync(userRegistration.Token, userRegistration.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponseDTO
                {
                    Errors = authResponse.Errors
                });

            }

            return Ok(new AuthSuccessResponseDTO { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });
        }
    }
}
