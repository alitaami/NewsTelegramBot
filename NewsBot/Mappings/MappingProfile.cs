using AutoMapper;
using NewsBot.Entities;
using NewsBot.Models.ViewModels;

namespace NewsBot.Mappings
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<NewsViewModel,News>().ReverseMap();
            CreateMap<NewsUpdateViewModel,News>().ReverseMap();
        }
    }
}
