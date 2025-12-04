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
    public class MessageRepository : PostgreSQLEfCoreRepositoryBase<MessageEntity, long>, IMessageRepository
    {
        protected readonly PostgreSQLDbContext _context;
        public MessageRepository(PostgreSQLDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _context = context;
        }

        public Task<MessageEntity> AddMessageAsync(MessageEntity msg)
        {
            throw new NotImplementedException();
        }
    }
}
