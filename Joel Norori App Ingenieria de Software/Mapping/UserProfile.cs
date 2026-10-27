using AutoMapper;
using Joel_Norori_App_Ingenieria_de_Software.Models;
namespace Joel_Norori_App_Ingenieria_de_Software.Mapping
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<Usuario, LoginReponseDto>()
                .ForMember(d => d.AccessToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.ExpireAt, opt => opt.Ignore());

            CreateMap<RegisterRequestDto, Usuario>()
                .ForMember(d => d.Password, opt => opt.Ignore())
                .ForMember(d => d.EmailConfirmed, opt => opt.MapFrom(src => false))
                .ForMember(d => d.EmailReConfirmToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshToken, opt => opt.Ignore())
                .ForMember(d => d.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(d => d.PasswordResetToken, opt => opt.Ignore())
                .ForMember(d => d.Role, opt => opt.Ignore());


        }

    }
}
