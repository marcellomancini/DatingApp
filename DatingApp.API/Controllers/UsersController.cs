using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var result = _mapper.Map<IEnumerable<UserListItem>>(users);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var result = _mapper.Map<UserDetail>(user);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdate userUpdate){
            
           var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (id != currentUserId)
            {
                return Unauthorized();
            }

            var dbUser = await _repo.GetUser(id);

            _mapper.Map(userUpdate, dbUser);

            if (await _repo.SaveAll())            
                return NoContent();
            
            throw new System.Exception($"Updating user {id} failed on save");

        }
    }
}