using System;
using CodePulse.Api.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.Api.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<BlogPost> BlogPosts { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<BlogImage> BlogImages { get; set; }
	}
}

