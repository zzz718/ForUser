using AutoMapper;
using AutoMapper.Internal.Mappers;
using ForUser.Application.Object.Dtos;
using ForUser.Domains.Commons.Object;
using ForUser.Domains.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Object
{
    public class ObjectService: IObjectService
    {
        private readonly IObjectRepository _objectRepository;
        private readonly IMapper _mapper;
        public ObjectService(IObjectRepository objectRepository, IMapper mapper)
        {
            _objectRepository = objectRepository;
            _mapper = mapper;
        }

        public async Task<ObjectView> GetObjectAsync(ObjectInput objectInput)
        {
            var objectEntity = await _objectRepository.FindAsync(x=>x.Id == objectInput.Id||x.Code == objectInput.Code);
            if (objectEntity == null)
            {
                throw new Exception("Object not found");
            }
            return _mapper.Map( objectEntity, new ObjectView());
        }

        public async Task CreateObjectAsync(CreateOrUpdateObjectDto objectDto)
        {
            var objectEntity = await _objectRepository.FindAsync( x=>x.Code == objectDto.Code);
            if (objectEntity != null)
            {
                throw new Exception("Object already exists");
            }
            ObjectEntity insertObject = _mapper.Map(objectDto,new ObjectEntity());
            await _objectRepository.AddAsync(insertObject);
            if (await _objectRepository.SaveAsync() > 0)
            {
            }
            else
            {
                throw new Exception("保存失败");
            }
        }
    }
}
