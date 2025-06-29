using System;
using CodePulse.Api.Data;
using CodePulse.Api.Models.Domain;
using CodePulse.Api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CodePulse.Api.Repositories.Implementation
{
	public class BlogPostRepository : IBlogPostRepository
    {
        private const string BlogPostMemoryCacheKey = "BlogPostMemoryCacheKey";
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<BlogPostRepository> logger;
        private readonly IMemoryCache cache;

        public BlogPostRepository(ApplicationDbContext dbContext, 
            ILogger<BlogPostRepository> logger,
            IMemoryCache cache)
        {
            applicationDbContext = dbContext;
            this.logger = logger;
            this.cache = cache;
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await applicationDbContext.BlogPosts.AddAsync(blogPost);
            await applicationDbContext.SaveChangesAsync();
            cache.Remove(BlogPostMemoryCacheKey);
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var blogpost = await applicationDbContext.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);

            if (blogpost is not null)
            {
                applicationDbContext.BlogPosts.Remove(blogpost);
                await applicationDbContext.SaveChangesAsync();
                cache.Remove(BlogPostMemoryCacheKey);
            }
            return blogpost;
        }

        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync()
        {
            bool cacheFound = cache.TryGetValue(BlogPostMemoryCacheKey, out List<BlogPost>? posts);
            if (cacheFound && posts != null) 
            {
                logger.LogInformation("Cache available. BlogPosts return from cache");
                return posts;
            }
            posts = await applicationDbContext.BlogPosts.Include(x=>x.Categories).ToListAsync();
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions();
            cacheEntryOptions.Priority = CacheItemPriority.High;
            cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            logger.LogInformation("Cache is set. BlogPosts return from database");
            cache.Set(BlogPostMemoryCacheKey, posts, cacheEntryOptions);

            return posts;
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await applicationDbContext.BlogPosts.Include(t => t.Categories).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await applicationDbContext.BlogPosts.Include(t => t.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await applicationDbContext.BlogPosts.Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlogPost is null)
            {
                return null;
            }

            //update blogpost
            applicationDbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);

            //update categories
            existingBlogPost.Categories = blogPost.Categories;

            await applicationDbContext.SaveChangesAsync();
            cache.Remove(BlogPostMemoryCacheKey);
            return blogPost;
        }
    }
}

