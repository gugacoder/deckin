using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Keep.Paper.Design.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;

namespace Keep.Paper.Design.Spec
{
  public class RefSpec
  {
    public string UserType { get; set; }

    public Collection<string> Keys { get; set; }

    public override string ToString()
    {
      var args = Keys?.Any() == true ? $"({string.Join(";", Keys)})" : "";
      return $"{UserType}{args}";
    }

    #region Conversões implícitas

    public static implicit operator string(RefSpec spec) => spec?.ToString();

    public static implicit operator RefSpec(string spec) => RefSpec.Parse(spec);

    #endregion

    #region Fábricas

    public static RefSpec ExtractFrom(Ref @ref)
    {
      return new RefSpec
      {
        UserType = @ref.UserType,
        Keys = new Collection<string>(@ref.Args.Keys ?? Enumerable.Empty<string>())
      };
    }

    public static RefSpec ExtractFrom(MethodInfo method)
      => ExtractFrom(null, method);

    public static RefSpec ExtractFrom(Type type, MethodInfo method)
    {
      var spec = new RefSpec();

      var designType = BaseTypeAttribute.GetBaseTypeOfEntity(method.ReturnType);
      var attr = designType?._Attribute<BaseTypeAttribute>();

      var parameterNames = GetSpecParameterNames(method);

      spec.UserType = DesignAttribute.NameMethod(type, method);
      spec.Keys = parameterNames.ToCollection();

      return spec;
    }

    public static IEnumerable<ParameterInfo> GetSpecParameters(MethodInfo method)
      => from parameter in method.GetParameters()
         where Is.Primitive(parameter.ParameterType)
         select parameter;

    public static IEnumerable<string> GetSpecParameterNames(MethodInfo method)
      => from parameter in method.GetParameters()
         where Is.Primitive(parameter.ParameterType)
         select parameter.Name.ToPascalCase();

    #endregion

    #region Parsers

    public static RefSpec Parse(string path)
    {
      try
      {
        if (path == null) return null;

        var tokens = path.Split('(', ')').NotNullOrEmpty();
        var typePart = tokens.First();
        var argsPart = tokens.Skip(1).FirstOrDefault() ?? "";

        var parts = typePart.Split('/').NotNullOrEmpty().ToArray();
        var type = parts.First();
        var subType = parts.Length > 1 ? string.Join("/", parts.Skip(1)) : null;

        var keys = new Collection<string>(
          from arg in argsPart.Split(';')
          where !string.IsNullOrEmpty(arg)
          let pair = arg.Split('=')
          let key = pair.First().Trim()
          let value = string.Join("=", pair.Skip(1).EmptyIfNull()).Trim()
          select key
        );

        return new RefSpec
        {
          UserType = subType,
          Keys = keys
        };
      }
      catch (Exception ex)
      {
        throw new Exception(
          $"O caminho é inválido para identificar uma ação: {path}", ex);
      }
    }

    public static bool TryParse(string path, out RefSpec spec)
    {
      try
      {
        spec = Parse(path);
        return true;
      }
      catch
      {
        spec = null;
        return false;
      }
    }

    #endregion
  }
}
