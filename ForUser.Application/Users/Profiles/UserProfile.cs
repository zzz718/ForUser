using AutoMapper;
using ForUser.Application.Users.Dtos;
using ForUser.Domains.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Users.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<CreateOrUpdateUserDto,UserEntity>();
            CreateMap<UserEntity, PageUserDto>();
            CreateMap<UserEntity, ViewUserDto>();
        }
    }
}
