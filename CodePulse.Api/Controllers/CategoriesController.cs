using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodePulse.Api.Data;
using CodePulse.Api.Models.Domain;
using CodePulse.Api.Models.DTO;
using CodePulse.Api.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CodePulse.Api.Controllers
{
    // https://localhost:xxxx/api/categories
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<CategoriesController> logger;
        public CategoriesController(ILogger<CategoriesController> logger, ICategoryRepository repository,
            IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        // GET: /api/<controller>?query=html&sortBy=name&sortDirection=desc
        public async Task<IActionResult> GetAllCategories([FromQuery] string? query, 
            [FromQuery] string? sortBy, [FromQuery] string? sortDirection,
            [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var categories = await repository.GetCategoriesAsync(
                query, sortBy, sortDirection, pageNumber, pageSize);

            // Map domain model to DTOs
            var response = mapper.Map<List<CategoryDTO>>(categories);

            
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        // GET: /api/<controller>/{Id}
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var existingCategory = await repository.GetById(id);

            if (existingCategory is null)
            {
                return NotFound();
            }

            var response = mapper.Map<CategoryDTO>(existingCategory);
            return Ok(response);
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetCount()
        {
            return Ok(await repository.GetCount());
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        // POST: /api/<controller>
        public async Task<IActionResult> CreateCategory(CreateCategoryRequestDTO request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Bad category object");
                }
                // Map DTO to domain model
                var category = mapper.Map<Category>(request);
                //var category = new Category()
                //{
                //    Name = request.Name,
                //    UrlHandle = request.UrlHandle
                //};
                category = await repository.CreateAsync(category);

                // Domain model to DTO
                var response = mapper.Map<CategoryDTO> (category);
                //var response = new CategoryDTO()
                //{
                //    Id = category.Id,
                //    Name = category.Name,
                //    UrlHandle = category.UrlHandle
                //};
                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while creating category");
                return StatusCode(500, "Error while creating category");
            }
        }

        // PUT: /api/<controller>/{Id}
        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, UpdateCategoryRequestDTO updateCategoryRequest)
        {
            // Convert DTO to Domain
            var category = mapper.Map<Category>(updateCategoryRequest);
            category.Id = id;
            //var category = new Category()
            //{
            //    Id = id,
            //    Name = updateCategoryRequest.Name,
            //    UrlHandle = updateCategoryRequest.UrlHandle
            //};

            category = await repository.UpdateAsync(category);
            if (category == null)
            {
                return NotFound();
            }

            var response = mapper.Map<CategoryDTO>(category);
            response.Id = id;

            //var response = new CategoryDTO
            //{
            //    Id = id,
            //    Name = category.Name,
            //    UrlHandle = category.UrlHandle
            //};
            return Ok(response);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        // DELETE: /api/<controller>/{Id}
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var existingCategory = await repository.DeleteAsync(id);

            if (existingCategory is null)
            {
                return NotFound();
            }

            var response = mapper.Map<CategoryDTO>(existingCategory);

            //var response = new CategoryDTO
            //{
            //    Id = existingCategory.Id,
            //    Name = existingCategory.Name,
            //    UrlHandle = existingCategory.UrlHandle
            //};
            return Ok(response);
        }
    }
}

