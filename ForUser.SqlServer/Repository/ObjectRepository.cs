using ForUser.Domains.Commons.Object;
using ForUser.Domains.Commons.UserRole;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.SqlServer.Repository
{
    public class ObjectRepository : SqlServerEfCoreRepositoryBase<ObjectEntity, long>, IObjectRepository
    {
        public ObjectRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,  httpContextAccessor)
        {
        }
    }
}
