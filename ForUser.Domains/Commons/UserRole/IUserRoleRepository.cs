using ForUser.Domains.Commons.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.UserRole
{
    public interface IUserRoleRepository : IRepository<UserRoleEntity, long>
    {
    }
}
