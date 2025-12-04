using ForUser.Domains.Kernels.Entities;
using ForUser.Domains.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ForUser.PostgreSQL.Repository
{
    public class ConversationRepository : PostgreSQLEfCoreRepositoryBase<ConversationEntity, long>, IConversationRepository
    {
        public ConversationRepository(PostgreSQLDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }

    }
}
