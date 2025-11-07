using AutoMapper;
using ForUser.Application.Object.Dtos;
using ForUser.Domains.Commons.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Object.Profiles
{
    public class ObjectProfile:Profile
    {
        public ObjectProfile()
        {
            CreateMap<CreateOrUpdateObjectDto, ObjectEntity>();
            CreateMap< ObjectEntity, ObjectView>();
        }
    }
}
