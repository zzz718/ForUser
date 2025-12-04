using ForUser.Domains.Commons.ObjectFunc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class ObjectFuncRepository : SqlServerEfCoreRepositoryBase<ObjectFuncEntity, long>, IObjectFuncRepository
    {
        public ObjectFuncRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
