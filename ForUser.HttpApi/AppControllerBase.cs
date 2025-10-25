using Microsoft.AspNetCore.Mvc;

namespace ForUser.HttpApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class AppControllerBase : ControllerBase
    {
    }
}
