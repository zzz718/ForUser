using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.UserRole
{
    public class UserRoleEntity : IEntity
    {
        public virtual long Id { get; protected set; }
        public UserRoleEntity() { }

        public UserRoleEntity(long Id)  { this.Id = Id; }

        public long UserId { get; set; }

        public long RoleId { get; set; }
    }
}
