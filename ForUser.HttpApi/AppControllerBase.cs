using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace ForUser.HttpApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Authorize(policy: "Permission")]
    public class AppControllerBase : ControllerBase
    {
    }
}
