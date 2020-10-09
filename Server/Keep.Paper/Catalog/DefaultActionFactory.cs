using System;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Types = Keep.Paper.Api.Types;
using Keep.Paper.Rendering;
using Keep.Tools;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Keep.Paper.Catalog
{
  [Expose]
  public class DefaultActionFactory : IActionFactory
  {
    public async Task<ICollection<IAction>> CreateActionsAsync()
    {
      var action = MethodAction.Create(this, nameof(EchoAsync));
      return await Task.FromResult(new[] { action });
    }

    [Action("Echo(*)")]
    public async Task<object> EchoAsync(IPath path, IPathArgs args)
    {
      return await Task.FromResult(new Types.Status
      {
        Data = new Types.Data(new
        {
          path,
          args
        }),
        Props = new Types.Status.Info
        {
          Reason = "Ok",
          Severity = Severity.Success
        }
      });
    }
  }
}
