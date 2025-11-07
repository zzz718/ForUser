using ForUser.Domains.Commons.RoleObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class RoleObjectRepository : EfCoreRepositoryBase<RoleObjectEntity, long>, IRoleObjectRepository
    {
        public RoleObjectRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
