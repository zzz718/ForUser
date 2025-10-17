using ForUser.Domains.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.User
{
    public class UserRepository : EfCoreRepositoryBase<UserEntity, long>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
