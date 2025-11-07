using ForUser.Application.Object.Dtos;
using ForUser.Domains.Commons.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Object
{
    public interface IObjectService
    {
        Task<ObjectView> GetObjectAsync(ObjectInput objectInput);
        Task CreateObjectAsync(CreateOrUpdateObjectDto objectDto);
    }
}
