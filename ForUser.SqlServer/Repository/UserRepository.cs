using ForUser.Domains.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class UserRepository : SqlServerEfCoreRepositoryBase<UserEntity, long>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,  httpContextAccessor)
        {
        }


    }
}
