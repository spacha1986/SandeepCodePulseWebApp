using System;
using CodePulse.Api.Models.Domain;

namespace CodePulse.Api.Repositories.Interface
{
	public interface IBlogPostRepository
	{
        Task<BlogPost> CreateAsync(BlogPost category);

        Task<IEnumerable<BlogPost>> GetBlogPostsAsync();

        Task<BlogPost?> GetByIdAsync(Guid id);

        Task<BlogPost?> GetByUrlHandleAsync(string urlHandle);

        Task<BlogPost?> UpdateAsync(BlogPost category);

        Task<BlogPost?> DeleteAsync(Guid id);
    }
}

