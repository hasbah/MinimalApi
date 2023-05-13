using AutoMapper;
using MinimalApi.Data.DTO;
using MinimalApi.Data.Entity;

namespace MinimalApi.Core
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<RegistrationRequestDTO, ApplicationUser>()
                    .ForMember(x => x.UserName, opt => opt.MapFrom(o => o.Email))
                    .ForMember(x => x.NormalizedUserName, opt => opt.MapFrom(o => o.Email.ToUpper()))
                    .ForMember(x => x.NormalizedEmail, opt => opt.MapFrom(o => o.Email.ToUpper()))
                    .ForMember(x => x.DateOfRegistration, opt => opt.MapFrom(o => DateTime.UtcNow));
        }
    }
}
