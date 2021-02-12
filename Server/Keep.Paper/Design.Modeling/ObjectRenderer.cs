using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Paper.Design.Rendering;
using Keep.Paper.Design.Spec;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.Design.Modeling
{
  [Expose]
  public class ObjectRenderer : IDesignRenderer
  {
    private HashMap<Entry> catalog;
    private readonly IServiceProvider services;

    public ObjectRenderer(IServiceProvider services)
    {
      this.catalog = LoadExposedPapers();
      this.services = services;
    }

    private HashMap<Entry> LoadExposedPapers()
    {
      var papers = new HashMap<Entry>();
      var types = ExposedTypes.GetTypes<IPaperDesign>();
      var flags = BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy;

      var entries =
        from type in types
        from method in type.GetMethods(flags)
        where typeof(IDesign).IsAssignableFrom(method.ReturnType)
           || typeof(Task<IDesign>).IsAssignableFrom(method.ReturnType)
        select new Entry { Type = type, Method = method };

      foreach (var entry in entries)
      {
        var spec = RefSpec.ExtractFrom(entry.Type, entry.Method);
        Debug.WriteLine($"/Api/1/{spec}");

        spec.Keys = null;

        papers.Add(spec, entry);
      }

      return papers;
    }

    public async Task RenderAsync(IDesignContext ctx, IRequest req,
      IOutput @out, NextAsync next)
    {
      var entry = catalog[req.Target.UserType];
      if (entry == null)
      {
        await next.Invoke(ctx, req, @out);
        return;
      }

      var instance = (IPaperDesign)services.Instantiate(entry.Type);
      var method = entry.Method;

      var form = req.Forms?.FirstOrDefault() ?? new Form();

      var parameters = method.GetParameters();
      var values = new object[parameters.Length];

      var args = req.Target?.Args?.ToArray() ?? new KeyValuePair<string, string>[0];

      for (int i = 0; i < parameters.Length; i++)
      {
        var name = parameters[i].Name;
        var type = parameters[i].ParameterType;
        if (typeof(Form).IsAssignableFrom(type))
        {
          values[i] = form;
        }
        else if (typeof(IRequest).IsAssignableFrom(type)
          || typeof(Request).IsAssignableFrom(type))
        {
          values[i] = req;
        }
        else if (Is.Primitive(type))
        {
          object value = null;

          var arg = args.FirstOrDefault(x => x.Key.EqualsAnyIgnoreCase(name));
          if (arg.Key != null)
          {
            args = args.Except(arg).ToArray();
            value = Change.To(arg.Value, type);
          }

          values[i] = Change.To(value, type);
        }
        else
        {
          throw new NotSupportedException();
        }
      }

      var result = method.Invoke(instance, values);
      var design = (IDesign)result;

      var response = design as IResponse;
      if (response == null)
        throw new NotSupportedException(
          $"Tipo de responta ainda não suportado: {design.GetType().FullName}");

      response.Entity.Self ??= req.Target;

      await @out.WriteAsync(response);
    }

    public class Entry
    {
      public Type Type { get; set; }
      public MethodInfo Method { get; set; }
    }
  }
}
