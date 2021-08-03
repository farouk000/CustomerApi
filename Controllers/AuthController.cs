using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CustomerApi.Data;
using CustomerApi.Models;
using CustomerApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly CustomerApiContext customerContext;
        readonly ITokenService tokenService;
        public AuthController(CustomerApiContext userContext, ITokenService tokenService)
        {
            this.customerContext = userContext ?? throw new ArgumentNullException(nameof(customerContext));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] User loginModel)
        {
            if (loginModel == null)
            {
                return BadRequest("Invalid client request");
            }
            var user = customerContext.User
                .FirstOrDefault(u => (u.Email == loginModel.Email) &&
                                        (u.Password == loginModel.Password));
            if (user == null)
            {
                return Unauthorized();
            }
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginModel.Email),
        };
            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            customerContext.SaveChanges();
            return Ok(user);
        }
    }
}
