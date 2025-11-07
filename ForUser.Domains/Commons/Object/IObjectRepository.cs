using ForUser.Domains.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.Object
{
    public interface IObjectRepository : IRepository<ObjectEntity, long>
    {
    }
}
