using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Keep.Paper.Security;
using System.Security.Claims;

namespace Keep.Paper.Controllers
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
      return Ok(new
      {
        userContext.Username,
        userContext.Domain
      });
    }
  }
}
