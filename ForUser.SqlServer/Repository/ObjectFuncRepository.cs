using ForUser.Domains.Commons.ObjectFunc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class ObjectFuncRepository : EfCoreRepositoryBase<ObjectFuncEntity, long>, IObjectFuncRepository
    {
        public ObjectFuncRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
