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
      var flags = BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.Static
                | BindingFlags.Instance;
      var method = type.GetMethod(methodName, flags);
      if (method == null)
        throw new Exception($"Método não encontrado: {type.FullName}:{methodName}");

      return Create(method);
    }

    public static MethodAction Create(MethodInfo method)
    {
      var collection = NameCollection(method.DeclaringType);
      var action = NameAction(method);

      var inferArgs = action.Contains("(...)");
      if (inferArgs)
      {
        action = action.Replace("(...)", "");

        var keys =
          from parameter in method.GetParameters()
          let parameterType = parameter.ParameterType
          where parameterType.IsValueType
             || Is.OfType<string>(parameterType)
             || Is.Var(parameterType)
          select parameter.Name.ToCamelCase();

        var hasAny = (
          from parameter in method.GetParameters()
          let parameterType = parameter.ParameterType
          where Is.OfType<IPathArgs>(parameterType)
          select parameter).Any();
        if (hasAny)
        {
          keys = keys.Append("*");
        }

        if (keys.Any())
        {
          action += $"({string.Join(";", keys)})";
        }
      }

      var pathName = action;
      if (pathName.StartsWith("."))
      {
        pathName = $"{collection}{action}";
      }

      var path = Catalog.Path.Parse(pathName);

      return new MethodAction(path, method);
    }

    private static string NameCollection(Type type)
    {
      var attr = type._Attribute<CollectionAttribute>();

      attr?.Validate();

      if (!string.IsNullOrEmpty(attr?.Name))
        return attr.Name;

      var name = type.FullName.Split(';', ',').First();

      if (name.EndsWith("Factory"))
      {
        name = name.Remove(name.Length - "Factory".Length);
      }
      if (name.EndsWith("Action"))
      {
        name = name.Remove(name.Length - "Action".Length);
      }
      if (name.EndsWith("Paper"))
      {
        name = name.Remove(name.Length - "Paper".Length);
      }

      return name;
    }

    private static string NameAction(MethodInfo method)
    {
      var attr = method._Attribute<ActionAttribute>();

      attr?.Validate();

      if (!string.IsNullOrEmpty(attr?.Pattern))
        return attr.Pattern;

      var name = method.Name;
      if (name.EndsWith("Async"))
      {
        name = name.Remove(name.Length - "Async".Length);
      }

      var action = $".{name}(...)";
      return action;
    }
  }
}
