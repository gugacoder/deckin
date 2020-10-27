using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Keep.Paper.Api;
using Keep.Paper.Catalog;
using Microsoft.Extensions.DependencyInjection;
using Types = Keep.Paper.Api.Types;

namespace Keep.Paper.Rendering
{
  public class MethodRenderer : IRenderer
  {
    private readonly MethodInfo method;
    private readonly Func<IServiceProvider, object> typeFactory;

    public MethodRenderer(MethodInfo method,
      Func<IServiceProvider, object> typeFactory = null)
    {
      this.method = method;
      this.typeFactory = typeFactory ??
        new Func<IServiceProvider, object>(
          provider => provider.Instantiate(method.DeclaringType));
    }

    public async Task<object> RenderAsync(IRenderingContext ctx,
      CancellationToken stopToken, RenderingChain next)
    {
      var matcher = new ParameterMatcher();
      var ret = await matcher.TryMatchParametersAsync(ctx, method, stopToken);

      if (!ret.Ok)
        return new Api.Types.Status
        {
          Props = new Api.Types.Status.Info
          {
            Fault = Fault.Failure,
            Reason = ret.Fault.Message
          }
        };

      var args = ret.Value;

      object host = null;

      if (!method.IsStatic)
      {
        var type = method.DeclaringType;
        host = typeFactory.Invoke(ctx.ServiceProvider);
      }

      var result = method.Invoke(host, args);
      if (result is Task task)
      {
        await task;
        result = ((dynamic)task).Result;
      }

      return result;
    }
  }
}
