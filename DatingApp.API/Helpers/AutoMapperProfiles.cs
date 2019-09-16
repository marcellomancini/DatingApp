using System;
using System.Linq;
using AutoMapper;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserDetail>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => GetPhotosUrl(src.Photos.FirstOrDefault(p => p.IsMain))))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.BirthDate.GetAge()));            
            CreateMap<User,UserListItem>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => GetPhotosUrl(src.Photos.FirstOrDefault(p => p.IsMain))))
              .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.BirthDate.GetAge()));
            CreateMap<Photo,UserDetailPhoto>();
             CreateMap<PhotoCreate,Photo>();
            CreateMap<UserUpdate,User>();
        }

        private string GetPhotosUrl(Photo p){
            return p?.Url;
        }
    }
} 