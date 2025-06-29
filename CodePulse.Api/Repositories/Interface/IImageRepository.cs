using System;
using CodePulse.Api.Models.Domain;

namespace CodePulse.Api.Repositories.Interface
{
	public interface IImageRepository
	{
		Task<BlogImage> Upload(IFormFile file, BlogImage blogImage);
		Task<BlogImage?> GetImage(Guid id);
		Task<IEnumerable<BlogImage>> GetAllImages();
    }
}

