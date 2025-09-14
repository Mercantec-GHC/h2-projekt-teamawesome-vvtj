using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Services;
using DomainModels;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Common;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveDirectoryController : ControllerBase
    {
        
        private readonly ActiveDirectoryService _activeDirectoryService;
       
        public ActiveDirectoryController(ActiveDirectoryService activeDirectoryService)
        {
            _activeDirectoryService = activeDirectoryService;
        }
        
        /// <summary>
        /// Authenticates a user via the AD and returns a JWT if successful
        /// </summary>
        /// <param name="ADdto">DTO containing the username and password from thr query strimg</param>
        /// <returns>OK with JWT if authentication succeeds; otherwise null</returns>
        [HttpGet]
        public async Task<IActionResult> ADLogin([FromQuery]ADLoginDto ADdto)
        {
            var adUser = await _activeDirectoryService.AuthenticateUserAsync(ADdto.Username, ADdto.Password);
            if (adUser == null)
                return null;
            var token = _activeDirectoryService.GenerateTokenForADUser(adUser);

            return Ok(new
            {
                token
            });
        }

    }
}
