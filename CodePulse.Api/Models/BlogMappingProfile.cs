using AutoMapper;
using CodePulse.Api.Models.Domain;
using CodePulse.Api.Models.DTO;

namespace CodePulse.Api.Models
{
    public class BlogMappingProfile : Profile
    {
        public BlogMappingProfile()
        {
            CreateMap<Category, CategoryDTO>();

            CreateMap<BlogPost, BlogPostDTO>();
            CreateMap<BlogImage, BlogImageDTO>();
            CreateMap<CreateCategoryRequestDTO, Category>();
            CreateMap<UpdateCategoryRequestDTO, Category>();

            CreateMap<CreateBlogPostRequestDTO, BlogPost>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

            CreateMap<UpdateBlogPostRequestDTO, BlogPost>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
