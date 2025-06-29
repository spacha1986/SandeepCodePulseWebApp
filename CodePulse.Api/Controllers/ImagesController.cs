using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodePulse.Api.Models.Domain;
using CodePulse.Api.Models.DTO;
using CodePulse.Api.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CodePulse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IImageRepository repository;

        public ImagesController(IImageRepository imageRepository)
        {
            repository = imageRepository;
        }

        // GET: /api/Images/{id}
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetImageById([FromRoute] Guid id)
        {
            var blogImage = await repository.GetImage(id);

            if (blogImage == null)
            {
                return NotFound();
            }

            var blogImageDto = new BlogImageDTO()
            {
                Id = blogImage.Id,
                FileName = blogImage.FileName,
                FileExtension = blogImage.FileExtension,
                Title = blogImage.Title,
                DateCreated = blogImage.DateCreated,
                Url = blogImage.Url
            };

            return Ok(blogImageDto);
        }

        // GET: /api/Images
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            var blogImages = await repository.GetAllImages();

            var blogImageDTOs = new List<BlogImageDTO>();
            foreach (var blogImage in blogImages)
            {

                var blogImageDto = new BlogImageDTO()
                {
                    Id = blogImage.Id,
                    FileName = blogImage.FileName,
                    FileExtension = blogImage.FileExtension,
                    Title = blogImage.Title,
                    DateCreated = blogImage.DateCreated,
                    Url = blogImage.Url
                };
                blogImageDTOs.Add(blogImageDto);
            }

            return Ok(blogImageDTOs);
        }

        // POST:/api/Images
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file,
            [FromForm] string fileName, [FromForm] string title)
        {
            ValidateFileUpload(file);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //File Upload
            var blogImage = new BlogImage
            {
                FileExtension = Path.GetExtension(file.FileName).ToLower(),
                FileName = fileName,
                Title = title,
                DateCreated = DateTime.Now
            };

            blogImage = await repository.Upload(file, blogImage);

            var blogImageDTO = new BlogImageDTO
            {
                Id = blogImage.Id,
                FileName = blogImage.FileName,
                FileExtension = blogImage.FileExtension,
                Title = blogImage.Title,
                DateCreated = blogImage.DateCreated,
                Url = blogImage.Url
            };

            return Ok(blogImageDTO);
        }


        private void ValidateFileUpload(IFormFile file)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file format");
            }
            if (file.Length > 10485760 /*10 MB*/)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10 MB");
            }
        }
    }
}

