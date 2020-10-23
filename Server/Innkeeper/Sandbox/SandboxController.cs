#if DEBUG
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Keep.Paper.Security;
using System.Security.Claims;
using Keep.Paper.Controllers;
using Keep.Paper.Api.Types;
using Keep.Paper.Types;

namespace Innkeeper.Sandbox
{
  [NewtonsoftJsonFormatter]
  public class SandboxController : Controller
  {
    private readonly IUserContext userContext;

    public SandboxController(IUserContext userContext)
    {
      this.userContext = userContext;
    }

    [Route("/SayHi")]
    public IActionResult SayHi()
    {
      return Ok(new Entity
      {
        Data = new Data(new
        {
          userContext.User
        })
      });
    }
  }
}
#endif
