using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Hosting.Extensions;
using Keep.Paper.Design.Rendering;
using Keep.Tools;
using Keep.Tools.Collections;

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
      var types = ExposedTypes.GetTypes<IPaper>();
      var flags = BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy;

      var entries =
        from type in types
        from method in type.GetMethods(flags)
        where typeof(IDesign).IsAssignableFrom(method.ReturnType)
        select new Entry { Type = type, Method = method };

      foreach (var entry in entries)
      {
        var @ref = Ref.Create(entry.Method);
        papers.Add(@ref, entry);

        Debug.WriteLine(@ref);
      }

      return papers;
    }

    public async Task RenderAsync(IDesignContext ctx, IRequest req,
      IResponse res, NextAsync next)
    {
      var entry = catalog[req.Target];
      if (entry == null)
      {
        await next.Invoke(ctx, req, res);
        return;
      }

      var instance = (IPaper)services.Instantiate(entry.Type);
      var method = entry.Method;

      var form = req.Forms?.FirstOrDefault() ?? new Form();

      var argTypes = method.GetParameters();
      var args = new object[argTypes.Length];

      for (int i = 0; i < argTypes.Length; i++)
      {
        var type = argTypes[i].ParameterType;
        if (typeof(Form).IsAssignableFrom(type))
        {
          args[i] = form;
        }
        else
        {
          throw new NotSupportedException();
        }
      }

      var result = method.Invoke(instance, args);
      var design = (IDesign)result;

      if (design is Entity entity)
      {
        entity.Self ??= req.Target;
      }

      await res.WriteAsync(design);
    }

    public class Entry
    {
      public Type Type { get; set; }
      public MethodInfo Method { get; set; }
    }
  }
}
