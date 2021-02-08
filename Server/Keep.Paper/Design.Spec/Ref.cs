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
  public class Ref : IRef
  {
    public string BaseType { get; set; }

    public string UserType { get; set; }

    public StringMap Args { get; set; }

    public override string ToString()
    {
      var baseType = string.IsNullOrWhiteSpace(BaseType) ? "*" : BaseType.Trim();
      var userType = string.IsNullOrWhiteSpace(UserType) ? "" : $"/{UserType.Trim()}";
      var keys = Args?.Select(arg => $"{arg.Key}={arg.Value}");
      var args = keys?.Any() == true ? $"({string.Join(";", keys)})" : "";
      return $"{baseType}{userType}{args}";
    }

    #region Conversões implícitas

    public static implicit operator string(Ref @ref) => @ref?.ToString();

    public static implicit operator Ref(string @ref) => Ref.Parse(@ref);

    #endregion

    #region Fábricas

    public static Ref ForLocalReference(IEntity entity)
    {
      var map = new StringMap();
      map.Add("#Ref", Guid.NewGuid().ToString("B"));
      return new Ref
      {
        BaseType = entity.GetType().Name,
        Args = map
      };
    }

    public static Ref<T> ForLocalReference<T>(T entity)
      where T : class, IEntity
    {
      var map = new StringMap();
      map.Add("#Ref", Guid.NewGuid().ToString("B"));
      return new Ref<T>
      {
        BaseType = entity.GetType().Name,
        Args = map
      };
    }

    public static Ref For(string baseType, string userType, object args = null)
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
        BaseType = baseType,
        UserType = userType,
        Args = map
      };
    }

    public static Ref<T> For<T>(string userType, object args = null)
      where T : class, IEntity
      => For(typeof(T).Name, userType, args).CastTo<T>();

    public static Ref For(string baseType, string userType, StringMap args)
    {
      return new Ref
      {
        BaseType = baseType,
        UserType = userType,
        Args = args ?? new StringMap()
      };
    }

    public static Ref<T> For<T>(string userType, StringMap args)
      where T : class, IEntity
      => For(typeof(T).Name, userType, args).CastTo<T>();

    #endregion

    #region Parsers

    public static Ref Parse(string path)
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

        var args = new StringMap(
          from arg in argsPart.Split(';')
          where !string.IsNullOrEmpty(arg)
          let pair = arg.Split('=')
          let key = pair.First().Trim()
          let value = string.Join("=", pair.Skip(1).EmptyIfNull()).Trim()
          select KeyValuePair.Create(key, value)
        );

        return new Ref
        {
          BaseType = type,
          UserType = subType,
          Args = args
        };
      }
      catch (Exception ex)
      {
        throw new Exception(
          $"O caminho é inválido para identificar uma ação: {path}", ex);
      }
    }

    public static Ref<T> Parse<T>(string path)
      where T : class, IEntity
      => Parse(path).CastTo<T>();

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

    public static bool TryParse<T>(string path, out Ref<T> @ref)
      where T : class, IEntity
    {
      var ok = TryParse(path, out Ref<T> @parsed);
      @ref  = @parsed.CastTo<T>();
      return ok;
    }

    #endregion
  }
}
