using ForUser.Domains.Kernels;
using ForUser.Domains.Kernels.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.PostgreSQL.Repository
{
    public class KnowLedgeRepository : PostgreSQLEfCoreRepositoryBase<EmbeddingEntity, long>, IKnowLedgeRepository
    {
        public KnowLedgeRepository(PostgreSQLDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,  httpContextAccessor)
        {
        }
    }
}
