using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.DTO.V1.Responses;
using TwitterBook.Models;

namespace TwitterBook.Mapping
{
    public class DomainToResponseProfile: Profile
    {
        public DomainToResponseProfile()
        {

            CreateMap<Tag, TagResponseDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TagString));

            CreateMap<Post, PostResponseDTO>();
                //.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => new TagResponseDTO { Name = src.Name }));
        }
    }
}
