using ForUser.Domains.Commons.RoleObject;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class RoleObjectRepository : SqlServerEfCoreRepositoryBase<RoleObjectEntity, long>, IRoleObjectRepository
    {
        public RoleObjectRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
