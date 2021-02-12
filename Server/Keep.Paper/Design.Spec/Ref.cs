using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Keep.Paper.Design.Modeling;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  [JsonConverter(typeof(RefConverter))]
  public class Ref
  {
    public string UserType { get; set; }

    public StringMap Args { get; set; }

    public override string ToString()
    {
      var keys = Args?.Select(arg => $"{arg.Key}={arg.Value}");
      var args = keys?.Any() == true ? $"({string.Join(";", keys)})" : "";
      return $"{UserType}{args}";
    }

    #region Conversões implícitas

    public static implicit operator string(Ref @ref) => @ref?.ToString();

    public static implicit operator Ref(string @ref) => Ref.Parse(@ref);

    #endregion

    #region Fábricas

    public static Ref For(string userType, StringMap args)
      => new Ref { UserType = userType, Args = args ?? new StringMap() };

    public static Ref For(string userType, object args = null)
    {
      var map = new StringMap();
      if (args != null)
      {
        map.AddMany(
          from entry in (args ?? new { })._Map()
          select KeyValuePair.Create(entry.Key, Change.To<string>(entry.Value))
        );
      }
      return new Ref
      {
        UserType = userType,
        Args = map
      };
    }

    public static Ref For(Type userType, string method, StringMap args)
      => For(DesignAttribute.NameMethod(userType, method), args);

    public static Ref For(Type userType, string method, object args = null)
      => For(DesignAttribute.NameMethod(userType, method), args);

    public static Ref For(Type userType, MethodInfo method, StringMap args)
      => For(DesignAttribute.NameMethod(userType, method), args);

    public static Ref For(Type userType, MethodInfo method, object args = null)
      => For(DesignAttribute.NameMethod(userType, method), args);

    public static Ref For(MethodInfo method, StringMap args)
      => For(DesignAttribute.NameMethod(method), args);

    public static Ref For(MethodInfo method, object args = null)
      => For(DesignAttribute.NameMethod(method), args);

    #endregion

    #region Parsers

    public static Ref Parse(string path)
    {
      try
      {
        if (path == null) return null;

        var tokens = path.Split('(', ')').NotNullOrEmpty();
        var typePart = tokens.First().Replace("/", "");
        var argsPart = tokens.Skip(1).FirstOrDefault() ?? "";

        var args = new StringMap(
          from arg in argsPart.Split(';')
          where !string.IsNullOrEmpty(arg)
          let pair = arg.Split('=')
          let key = (pair.Length > 1)
            ? pair.First().Trim()
            : throw new NotImplementedException($"O parâmetro deveria ter a forma `chave=valor`: {arg}")
          let value = string.Join("=", pair.Skip(1).EmptyIfNull()).Trim()
          select KeyValuePair.Create(key.ToPascalCase(), value)
        );

        return new Ref
        {
          UserType = typePart,
          Args = args
        };
      }
      catch (Exception ex)
      {
        throw new Exception(
          $"O caminho é inválido para identificar uma ação: {path}", ex);
      }
    }

    public static bool TryParse(string path, out Ref @ref)
    {
      try
      {
        @ref = Parse(path);
        return true;
      }
      catch
      {
        @ref = null;
        return false;
      }
    }

    #endregion
  }
}
