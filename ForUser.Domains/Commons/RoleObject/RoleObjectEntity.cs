using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.RoleObject
{
    public class RoleObjectEntity : IEntity
    {
        public virtual long Id { get; protected set; }

        public RoleObjectEntity() { }

        public RoleObjectEntity(long Id){ this.Id = Id; }

        public long RoleId { get; set; }
        public long ObjectId { get; set; }
    }
}
