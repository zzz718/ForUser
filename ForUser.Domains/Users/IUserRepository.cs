using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Users
{
    public interface IUserRepository:IRepository<UserEntity,long>
    {
    }
}
