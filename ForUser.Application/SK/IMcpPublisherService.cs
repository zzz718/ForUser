using ForUser.Application.SK.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForUser.Application.SK
{
    public interface IMcpPublisherService
    {
        Task<string> PublishAsync(MCPInvokeMessage message);
    }
}
