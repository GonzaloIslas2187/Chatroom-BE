using AutoMapper;
using Chatroom.DTOs;
using Chatroom.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace Chatroom.Mapper
{
    public class MapperProfile : Profile
    {
        private readonly IConfiguration _configuration;
        private readonly IDataProtectionProvider _dataProtectionProvider;


        public MapperProfile(IConfiguration configuration, IDataProtectionProvider dataProtectionProvider)
        {
            _configuration = configuration;
            _dataProtectionProvider = dataProtectionProvider;

            CreateMappings();
        }

        private void CreateMappings()
        {
            CreateMap<CredentialsDTO, Credential>()
                .ConstructUsing(x => new Credential(x.Email));

            CreateMap<Message, MessageDTO>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
        }

    }
}
