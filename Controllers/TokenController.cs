using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerApi.Data;
using CustomerApi.Models;
using CustomerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        readonly CustomerApiContext customerContext;
        readonly ITokenService tokenService;

        public TokenController(CustomerApiContext customerContext, ITokenService tokenService)
        {
            this.customerContext = customerContext ?? throw new ArgumentNullException(nameof(customerContext));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = customerContext.User.SingleOrDefault(u => u.Email == username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }

            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            customerContext.SaveChanges();

            return new ObjectResult(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;

            var user = customerContext.User.SingleOrDefault(u => u.Email == username);
            if (user == null) return BadRequest();

            user.RefreshToken = null;

            customerContext.SaveChanges();

            return NoContent();
        }
    }
}
