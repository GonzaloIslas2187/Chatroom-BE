using AutoMapper;
using Chatroom.Common;
using Chatroom.DTOs;
using Chatroom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Chatroom.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CredentialController : ControllerBase
    {
        private readonly UserManager<Credential> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CredentialController(UserManager<Credential> userManager, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(CredentialsDTO credentialDto)
        {
            var user = await _userManager.FindByEmailAsync(credentialDto.Email);

            if (user == null)
                return BadRequest("Password or Email Invalid");

            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, credentialDto.Password);

            if (!passwordCheckResult)
                return BadRequest("Password or Email Invalid");

            var token = await TokenHandler.CreateToken(user, _configuration);
            return Ok(new { token, userName = user.Email, id = user.Id });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(CredentialsDTO credentialDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(credentialDto.Email);
            if (existingUser != default)
                return BadRequest("Email is already in use.");

            var user = _mapper.Map<Credential>(credentialDto);
            var userCreateResult = await _userManager.CreateAsync(user, credentialDto.Password);

            if (!userCreateResult.Succeeded)
                return BadRequest(string.Join(" ", userCreateResult.Errors.Select(e => e.Description)));

            var token = await TokenHandler.CreateToken(user, _configuration);
            return Ok(new { token, userName = user.Email, id = user.Id });
        }
    }
}
