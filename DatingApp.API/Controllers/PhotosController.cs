using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _options;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> options)
        {
            this._options = options;
            this._mapper = mapper;
            this._repo = repo;

            Account acc = new Account(options.Value.CloudName, options.Value.ApiKey, options.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var dbPhoto = await _repo.GetPhoto(id);
            var photo = _mapper.Map<UserDetailPhoto>(dbPhoto);
            return Ok(photo);
        }



        [HttpPost]
        public async Task<IActionResult> CreateUserPhoto(int userId, [FromForm] PhotoCreate photo)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != currentUserId)
            {
                return Unauthorized();
            }

            var dbUser = await _repo.GetUser(userId);

            var file = photo.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photo.Url = uploadResult.Uri.ToString();
            photo.PublicId = uploadResult.PublicId;

            var dbPhoto = _mapper.Map<Photo>(photo);
            if (!dbUser.Photos.Any(u => u.IsMain))
            {
                dbPhoto.IsMain = true;
            }

            dbUser.Photos.Add(dbPhoto);


            if (await _repo.SaveAll())
            {
                var detail = _mapper.Map<UserDetailPhoto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = detail.Id }, detail);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != currentUserId)
            {
                return Unauthorized();
            }

            var dbUser = await _repo.GetUser(userId);

            if (!dbUser.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var dbPhoto = await _repo.GetPhoto(id);

            if (dbPhoto.IsMain)
                return BadRequest("This is already the main photo");

            var dbMainPhoto = await _repo.GetUserMainPhoto(userId);
            dbMainPhoto.IsMain = false;
            dbPhoto.IsMain = true;
            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo  to main");
        }


    }
}