using ForUser.Domains.Kernels.Entities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Kernels
{
    public interface IConversationRepository : IRepository<ConversationEntity, long>
    {
        //Task<ConversationEntity> CreateAsync(ConversationEntity conv);
        //Task<ConversationEntity?> GetByIdAsync(long conversationId);
        //Task<IEnumerable<ConversationEntity>> ListByUserAsync(long userId);
        //Task<bool> DeleteConversationAsync(long conversationId);

    }
}
