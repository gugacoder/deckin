using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Keep.Paper.Rendering;
using Keep.Tools;
using Keep.Tools.Reflection;

namespace Keep.Paper.Catalog
{
  public class MethodAction : IAction
  {
    private readonly MethodRenderer renderer;

    public MethodAction(IPath path, MethodInfo method)
    {
      this.Path = path;
      this.renderer = new MethodRenderer(method);
    }

    public IPath Path { get; }

    public async Task<object> RenderAsync(IRenderingContext ctx,
      RenderingChain next)
    {
      return await renderer.RenderAsync(ctx, next);
    }

    public static MethodAction Create(object typeOrObject, string methodName)
    {
      var type = typeOrObject as Type ?? typeOrObject.GetType();
      return Create(null, type, methodName);
    }

    public static MethodAction Create(string pathPattern, object typeOrObject, string methodName)
    {
      var type = typeOrObject as Type ?? typeOrObject.GetType();
      var flags = BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static
                | BindingFlags.Instance;
      var method = type.GetMethod(methodName, flags);
      if (method == null)
        throw new Exception($"Método não encontrado: {type.FullName}:{methodName}");

      return Create(pathPattern, method);
    }

    public static MethodAction Create(MethodInfo method)
    {
      return Create(null, method);
    }

    public static MethodAction Create(string pathPattern, MethodInfo method)
    {
      Catalog.Path path;
      if (!string.IsNullOrEmpty(pathPattern))
      {
        path = Catalog.Path.Parse(pathPattern);
      }
      else if (method._Attribute<ActionAttribute>() is ActionAttribute attr)
      {
        path = Catalog.Path.Parse(attr.Pattern);
      }
      else
      {
        var type = method.DeclaringType;
        var typeName = type.FullName.Split(';', ',').First();
        if (typeName.EndsWith("Factory"))
        {
          typeName = typeName.Remove(typeName.Length - "Factory".Length);
        }
        if (typeName.EndsWith("Action"))
        {
          typeName = typeName.Remove(typeName.Length - "Action".Length);
        }
        if (typeName.EndsWith("Paper"))
        {
          typeName = typeName.Remove(typeName.Length - "Paper".Length);
        }

        var methodName = method.Name;
        if (methodName.EndsWith("Async"))
        {
          methodName = methodName.Remove(methodName.Length - "Async".Length);
        }

        var name = $"{typeName}.{methodName}";
        var keys =
          from parameter in method.GetParameters()
          let parameterType = parameter.ParameterType
          where parameterType.IsValueType
             || Is.OfType<string>(parameterType)
             || Is.Var(parameterType)
          select parameter.Name.ToCamelCase();

        path = new Path
        {
          Name = name,
          Keys = keys.ToArray()
        };
      }
      return new MethodAction(path, method);
    }
  }
}
