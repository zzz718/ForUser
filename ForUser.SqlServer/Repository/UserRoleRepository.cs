using ForUser.Domains.Commons.UserRole;
using ForUser.Domains.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class UserRoleRepository : SqlServerEfCoreRepositoryBase<UserRoleEntity, long>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,  httpContextAccessor)
        {
        }
    }
}
