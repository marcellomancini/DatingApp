using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository authRepo, IConfiguration config)
        {
            _config = config;
            _authRepo = authRepo;
        }

        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register(Credentials credentials)
        {
            //validate request          
            if (await _authRepo.UserExists(credentials.UserName))
                return BadRequest("Username already exists");

            var user = new User
            {
                UserName = credentials.UserName
            };

            user = await _authRepo.Register(user, credentials.Password);
            return StatusCode(201);
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(Credentials credentials)
        {
            //validate request          

            var user = await _authRepo.Login(credentials.UserName, credentials.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var sc = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var std=new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials=sc
            };
            
            var jwtSTH = new JwtSecurityTokenHandler();
            var token = jwtSTH.CreateToken(std);

            return Ok
            (
                new{token=jwtSTH.WriteToken(token)}
            );
        

        }
    }
}