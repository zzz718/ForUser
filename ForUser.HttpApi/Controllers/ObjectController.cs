using ForUser.Application.Object;
using ForUser.Application.Object.Dtos;
using ForUser.Domains.Attributes;
using ForUser.Domains.Commons;
using ForUser.Domains.Commons.Object;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.HttpApi.Controllers
{
    [ApiExplorerSettings(GroupName = ModuleCode.Permission)]
    public class ObjectController:AppControllerBase
    {

        private readonly IObjectService _objectService;

        public ObjectController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        [HttpPost]
        public async Task<ObjectView> GetObject([FromBody]ObjectInput input)
        {
            return await _objectService.GetObjectAsync(input);
        }

        [HttpPost]
        [Permission("1","创建对象")]
        public async Task<string> CreateObject([FromBody] CreateOrUpdateObjectDto objectDto)
        {
            await _objectService.CreateObjectAsync(objectDto);
            return "success";
        }


    }
}
