using System;
using CodePulse.Api.Models.Domain;

namespace CodePulse.Api.Repositories.Interface
{
	public interface ICategoryRepository
	{
		Task<Category> CreateAsync(Category category);

        Task<IEnumerable<Category>> GetCategoriesAsync(string? query = null,
			string? sortBy = null, 
			string? sortDirection = null,
			int? pageNumber = 1,
			int? pageSize = 100);

		Task<Category?> GetById(Guid id);

		Task<Category?> UpdateAsync(Category category);

        Task<Category?> DeleteAsync(Guid id);

		Task<int> GetCount();
    }
}

