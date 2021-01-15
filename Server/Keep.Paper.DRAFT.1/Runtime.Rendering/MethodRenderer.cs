using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Paper.Design;
using Keep.Paper.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Types = Keep.Paper.Design.Modeling;

namespace Keep.Paper.Runtime.Rendering
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
        return new Design.Modeling.Status
        {
          Props = new Design.Modeling.Status.Info
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
