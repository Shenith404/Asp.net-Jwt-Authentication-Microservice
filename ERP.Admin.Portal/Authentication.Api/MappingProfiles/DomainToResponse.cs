using Authentication.Core.DTOs;
using AutoMapper;
using ERP.Authentication.Core.Entity;

namespace Authentication.Api.MappingProfiles
{
    public class DomainToResponse :Profile
    {
        public DomainToResponse()
        {
            CreateMap<UserModel, UserModelResponseDTO>()
            .ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => src.AddedDate))
            .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate));


          
        }
    }
}
