using System;
using CodePulse.Api.Data;
using CodePulse.Api.Models.Domain;
using CodePulse.Api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.Api.Repositories.Implementation
{
	public class ImageRepository :IImageRepository
	{
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ApplicationDbContext applicationDbContext;

        public ImageRepository(IWebHostEnvironment hostEnvironment, 
            IHttpContextAccessor contextAccessor, 
            ApplicationDbContext dbContext)
		{
            this.hostEnvironment = hostEnvironment;
            this.contextAccessor = contextAccessor;
            this.applicationDbContext = dbContext;
        }

        public async Task<IEnumerable<BlogImage>> GetAllImages()
        {
            return await applicationDbContext.BlogImages.ToListAsync();
        }

        public async Task<BlogImage?> GetImage(Guid id)
        {
            return await applicationDbContext.BlogImages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
        {
            // 1. Upload the image to api/image
            var localPath = Path.Combine(hostEnvironment.ContentRootPath, "Images", $"{blogImage.FileName}{blogImage.FileExtension}");

            var stream = new FileStream(localPath, FileMode.Create);

            await file.CopyToAsync(stream);



            // 2. Update to the database

            // Path to the image will be https://codepulse.api.com/images/somefilename.jpg

            var httpRequest = contextAccessor.HttpContext.Request;
            var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";
            
            blogImage.Url = urlPath;

            await applicationDbContext.BlogImages.AddAsync(blogImage);
            await applicationDbContext.SaveChangesAsync();

            return blogImage;

        }
    }
}

