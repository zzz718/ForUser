using ForUser.Domains.Kernels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels
{
    public interface IMessageRepository: IRepository<MessageEntity,long>
    {
        Task<MessageEntity> AddMessageAsync(MessageEntity msg);
    }
}
