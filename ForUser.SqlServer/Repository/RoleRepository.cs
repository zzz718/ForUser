using ForUser.Domains.Commons.Role;
using ForUser.Domains.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class RoleRepository : SqlServerEfCoreRepositoryBase<RoleEntity, long>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,  httpContextAccessor)
        {
        }
    }
}
