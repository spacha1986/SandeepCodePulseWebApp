using System;
using CodePulse.Api.Data;
using CodePulse.Api.Models.Domain;
using CodePulse.Api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace CodePulse.Api.Repositories.Implementation
{
	public class CategoryRepository : ICategoryRepository
    {
        private const string CategoryMemoryCacheKey = "CategoryMemoryCacheKey";
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<CategoryRepository> logger;
        private readonly IMemoryCache cache;

        public CategoryRepository(ApplicationDbContext dbContext, 
            ILogger<CategoryRepository> logger, IMemoryCache cache)
		{
            applicationDbContext = dbContext;
            this.logger = logger;
            this.cache = cache;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await applicationDbContext.Categories.AddAsync(category);
            await applicationDbContext.SaveChangesAsync();
            cache.Remove(CategoryMemoryCacheKey);
            return category;
        }

        public async Task<Category?> DeleteAsync(Guid id)
        {
            var category = await applicationDbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category is not null)
            {
                applicationDbContext.Categories.Remove(category);
                await applicationDbContext.SaveChangesAsync();
                cache.Remove(CategoryMemoryCacheKey);
            }
            return category;
        }

        public async Task<Category?> GetById(Guid id)
        {
            return await applicationDbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync(string? query = null,
            string? sortBy = null, 
            string? sortDirection = null,
            int? pageNumber = 1,
            int? pageSize = 100)
        {
            IQueryable<Category>? categories;
            bool cacheFound = cache.TryGetValue(CategoryMemoryCacheKey, out List<Category>? allCategories);
            if (!cacheFound || allCategories == null)
            {
                allCategories = await applicationDbContext.Categories.ToListAsync();
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions();
                cacheEntryOptions.Priority = CacheItemPriority.High;
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                logger.LogInformation("Cache is set. Categories return from database");
                cache.Set(CategoryMemoryCacheKey, allCategories, cacheEntryOptions);
            }
            else
            {
                logger.LogInformation("Cache available. Categories return from cache");
            }
            categories = allCategories.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(query))
            {
                logger.LogInformation("Searching Categories with Query : " + query);
                
                //Filtering
                categories = categories.Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
               
            }
            //Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                logger.LogInformation("Sorting Categories by : " + sortBy);
                categories = SortCatagory(sortBy, sortDirection, categories);
            }

            // Pagination
            // PageNumber 1, PageSize 5 - skip 0 , take 5
            // PageNumber 2, PageSize 5 - skip 5 , take 5
            // PageNumber 3, PageSize 5 - skip 10 , take 5

            var skipResults = (pageNumber - 1) * pageSize;

            categories = categories.Skip(skipResults??0).Take(pageSize ?? 100);

            return categories.ToList();
        }

        private IQueryable<Category> SortCatagory(string sortBy, string? sortDirection, IQueryable<Category> categories)
        {
            
            var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ?
                    true : false;

            if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
            {
                categories = isAsc? categories.OrderBy(x=>x.Name) : 
                    categories.OrderByDescending(x=>x.Name);
            }

            else if (string.Equals(sortBy, "url", StringComparison.OrdinalIgnoreCase))
            {
                categories = isAsc ? categories.OrderBy(x => x.UrlHandle) :
                    categories.OrderByDescending(x => x.UrlHandle);
            }

            return categories;
        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            var cat = await applicationDbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

            if (cat != null)
            {
                applicationDbContext.Entry(cat).CurrentValues.SetValues(category);
                await applicationDbContext.SaveChangesAsync();
                cache.Remove(CategoryMemoryCacheKey);
                return category;
            }
            return null; 
        }

        private List<string> GetPropertyNames(object obj)
        {
            List<string> propertyNames = new List<string>();
            var type = obj.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                propertyNames.Add(property.Name);
            }
            return propertyNames;
        }

        public async Task<int> GetCount()
        {
            return GetCategoriesAsync(pageSize:int.MaxValue).Result.Count();
        }
    }
}

