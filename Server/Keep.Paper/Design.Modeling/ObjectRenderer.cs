using System;
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
      var entry = catalog[req.Target];
      if (entry == null)
      {
        await next.Invoke(ctx, req, @out);
        return;
      }

      var instance = (IPaperDesign)services.Instantiate(entry.Type);
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

      var response = design as IResponse;
      if (response == null)
        throw new NotSupportedException(
          $"Tipo de responta ainda não suportado: {design.GetType().FullName}");

      response.Data.Self ??= req.Target;

      await @out.WriteAsync(response);
    }

    public class Entry
    {
      public Type Type { get; set; }
      public MethodInfo Method { get; set; }
    }
  }
}
