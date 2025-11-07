using ForUser.Domains.Commons.Object;
using ForUser.Domains.Commons.UserRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class ObjectRepository : EfCoreRepositoryBase<ObjectEntity, long>, IObjectRepository
    {
        public ObjectRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
