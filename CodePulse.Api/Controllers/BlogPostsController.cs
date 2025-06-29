using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using CodePulse.Api.Models.Domain;
using CodePulse.Api.Models.DTO;
using CodePulse.Api.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CodePulse.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : Controller
    {
        private readonly IBlogPostRepository repository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly ILogger<BlogPostsController> logger;

        public BlogPostsController(ILogger<BlogPostsController> logger, IBlogPostRepository repository,
            ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.repository = repository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.logger = logger;
        }


        [HttpGet]
        // GET: /api/<controller>/
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await repository.GetBlogPostsAsync();

            // Map domain model to DTOs
            var response = mapper.Map<List<BlogPostDTO>>(blogPosts);

            
            

            //var response = new List<BlogPostDTO>();
            //foreach (var blogPost in blogPosts)
            //{
            //    response.Add(new BlogPostDTO()
            //    {
            //        Id = blogPost.Id,
            //        Title = blogPost.Title,
            //        ShortDescription = blogPost.ShortDescription,
            //        UrlHandle = blogPost.UrlHandle,
            //        Content = blogPost.Content,
            //        FeaturedImageUrl = blogPost.FeaturedImageUrl,
            //        Author = blogPost.Author,
            //        IsVisible = blogPost.IsVisible,
            //        PublishedDate = blogPost.PublishedDate,
            //        Categories = blogPost.Categories.Select(x => new CategoryDTO
            //        {
            //            Id = x.Id,
            //            Name = x.Name,
            //            UrlHandle = x.UrlHandle
            //        }).ToList()
            //    });
            //}
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        // GET: /api/<controller>/{Id}
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            var blogPost = await repository.GetByIdAsync(id);

            if (blogPost is null)
            {
                return NotFound();
            }

            // Domain model to DTO
            var response = mapper.Map<BlogPostDTO>(blogPost);
            //var response = new BlogPostDTO()
            //{
            //    Id = blogPost.Id,
            //    Title = blogPost.Title,
            //    ShortDescription = blogPost.ShortDescription,
            //    UrlHandle = blogPost.UrlHandle,
            //    Content = blogPost.Content,
            //    FeaturedImageUrl = blogPost.FeaturedImageUrl,
            //    Author = blogPost.Author,
            //    IsVisible = blogPost.IsVisible,
            //    PublishedDate = blogPost.PublishedDate,
            //    Categories = blogPost.Categories.Select(x => new CategoryDTO
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        UrlHandle = x.UrlHandle
            //    }).ToList()
            //};
            return Ok(response);
        }

        [HttpGet("{urlHandle}")]
        // GET: /api/<controller>/{urlHandle}
        public async Task<IActionResult> GetBlogPostByUrl([FromRoute] string urlHandle)
        {
            // Get details from repository
            var blogPost = await repository.GetByUrlHandleAsync(urlHandle);

            if (blogPost is null)
            {
                return NotFound();
            }

            // Domain model to DTO

            var response = mapper.Map<BlogPostDTO>(blogPost);
            //var response = new BlogPostDTO()
            //{
            //    Id = blogPost.Id,
            //    Title = blogPost.Title,
            //    ShortDescription = blogPost.ShortDescription,
            //    UrlHandle = blogPost.UrlHandle,
            //    Content = blogPost.Content,
            //    FeaturedImageUrl = blogPost.FeaturedImageUrl,
            //    Author = blogPost.Author,
            //    IsVisible = blogPost.IsVisible,
            //    PublishedDate = blogPost.PublishedDate,
            //    Categories = blogPost.Categories.Select(x => new CategoryDTO
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        UrlHandle = x.UrlHandle
            //    }).ToList()
            //};
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        // POST: /api/<controller>
        public async Task<IActionResult> CreateBlogPost(CreateBlogPostRequestDTO request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Bad blogpost object");
                }
                // Map DTO to domain model
                var blogPost = mapper.Map<BlogPost>(request);
                //var blogPost = new BlogPost()
                //{
                //    Title = request.Title,
                //    ShortDescription = request.ShortDescription,
                //    UrlHandle = request.UrlHandle,
                //    Content = request.Content,
                //    FeaturedImageUrl = request.FeaturedImageUrl,
                //    Author = request.Author,
                //    IsVisible = request.IsVisible,
                //    PublishedDate = request.PublishedDate,
                //    Categories = new List<Category>()
                //};

                foreach (var categoryGuid in request.Categories)
                {
                    var existingCategory = await categoryRepository.GetById(categoryGuid);
                    if (existingCategory is not null)
                    {
                        blogPost.Categories.Add(existingCategory);
                    }
                }

                blogPost = await repository.CreateAsync(blogPost);

                // Domain model to DTO
                var response = mapper.Map<BlogPostDTO>(blogPost);
                //var response = new BlogPostDTO()
                //{
                //    Id = blogPost.Id,
                //    Title = blogPost.Title,
                //    ShortDescription = blogPost.ShortDescription,
                //    UrlHandle = blogPost.UrlHandle,
                //    Content = blogPost.Content,
                //    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                //    Author = blogPost.Author,
                //    IsVisible = blogPost.IsVisible,
                //    PublishedDate = blogPost.PublishedDate,
                //    Categories = blogPost.Categories.Select(x => new CategoryDTO
                //    {
                //        Id = x.Id,
                //        Name = x.Name,
                //        UrlHandle = x.UrlHandle
                //    }).ToList()
                //};
                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while creating blog post");
                return StatusCode(500, "Error while creating blog post");
            }
        }

        // PUT: /api/<controller>/{Id}
        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, UpdateBlogPostRequestDTO updateBlogPostRequest)
        {
            // Convert DTO to Domain
            var blogPost = mapper.Map<BlogPost>(updateBlogPostRequest);
            //var blogPost = new BlogPost()
            //{
            //    Id = id,
            //    Title = updateBlogPostRequest.Title,
            //    ShortDescription = updateBlogPostRequest.ShortDescription,
            //    UrlHandle = updateBlogPostRequest.UrlHandle,
            //    Content = updateBlogPostRequest.Content,
            //    FeaturedImageUrl = updateBlogPostRequest.FeaturedImageUrl,
            //    Author = updateBlogPostRequest.Author,
            //    IsVisible = updateBlogPostRequest.IsVisible,
            //    PublishedDate = updateBlogPostRequest.PublishedDate,
            //    Categories = new List<Category>()
            //};

            blogPost.Id = id;

            foreach (var categoryGuid in updateBlogPostRequest.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if (existingCategory is not null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await repository.UpdateAsync(blogPost);
            if (blogPost == null)
            {
                return NotFound();
            }

            var response = mapper.Map<BlogPostDTO>(blogPost);
            //var response = new BlogPostDTO()
            //{
            //    Id = blogPost.Id,
            //    Title = blogPost.Title,
            //    ShortDescription = blogPost.ShortDescription,
            //    UrlHandle = blogPost.UrlHandle,
            //    Content = blogPost.Content,
            //    FeaturedImageUrl = blogPost.FeaturedImageUrl,
            //    Author = blogPost.Author,
            //    IsVisible = blogPost.IsVisible,
            //    PublishedDate = blogPost.PublishedDate,
            //    Categories = blogPost.Categories.Select(x => new CategoryDTO
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        UrlHandle = x.UrlHandle
            //    }).ToList()
            //};
            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        // DELETE: /api/<controller>/{Id}
        public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
        {
            var existingBlogPost = await repository.DeleteAsync(id);

            if (existingBlogPost is null)
            {
                return NotFound();
            }

            var response = mapper.Map<BlogPostDTO>(existingBlogPost);

            //var response = new BlogPostDTO()
            //{
            //    Title = existingBlogPost.Title,
            //    ShortDescription = existingBlogPost.ShortDescription,
            //    UrlHandle = existingBlogPost.UrlHandle,
            //    Content = existingBlogPost.Content,
            //    FeaturedImageUrl = existingBlogPost.FeaturedImageUrl,
            //    Author = existingBlogPost.Author,
            //    IsVisible = existingBlogPost.IsVisible,
            //    PublishedDate = existingBlogPost.PublishedDate
            //};
            return Ok(response);
        }
    }
}

