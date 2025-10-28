using AutoMapper;
using MobileShopAPI.DTOs;
using MobileShopAPI.Models;

namespace MobileShopAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Brand mappings
            CreateMap<BrandDto, Brand>();
            CreateMap<Brand, BrandDto>();
            CreateMap<UpdateBrandDto, Brand>();
            
            // Model mappings
            CreateMap<ModelDto, Model>();
            CreateMap<Model, ModelDto>();
            CreateMap<UpdateModelDto, Model>();
        }
    }
}