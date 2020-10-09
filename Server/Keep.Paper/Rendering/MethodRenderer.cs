﻿using System;
using System.Linq;
using System.Reflection;
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

    public MethodRenderer(MethodInfo method)
    {
      this.method = method;
    }

    public async Task<object> RenderAsync(IRenderingContext ctx, RenderingChain next)
    {
      var matcher = new ParameterMatcher();
      var ret = await matcher.TryMatchParametersAsync(ctx, method);

      if (!ret.Ok)
        return new Types.Status
        {
          Props = new Types.Status.Info
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
        host = ActivatorUtilities.CreateInstance(ctx.ServiceProvider, type);
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