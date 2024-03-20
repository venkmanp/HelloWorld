using AutoMapper;
using HelloWorld.Models;

namespace HelloWorld.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductProfile, ProductDTO>();
        }

    }
}
