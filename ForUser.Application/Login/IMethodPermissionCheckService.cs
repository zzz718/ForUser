using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.Login
{
    public interface IMethodPermissionCheckService
    {
        Task<bool> CheckPermissionAync(string method);
    }
}
