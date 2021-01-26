using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [JsonConverter(typeof(RefConverter))]
  public class Ref
  {
    public static Ref Create(string type, string name)
    {
      return new Ref
      {
        Type = type,
        Name = name,
        Args = new StringMap()
      };
    }

    public static Ref Create(string type, string name, object args)
    {
      var entries =
        from entry in args._Map()
        select KeyValuePair.Create(entry.Key, Change.To<string>(entry.Value));
      return new Ref
      {
        Type = type,
        Name = name,
        Args = new StringMap(entries)
      };
    }

    public static Ref Create(string type, string name, StringMap args)
    {
      return new Ref
      {
        Type = type,
        Name = name,
        Args = args
      };
    }

    public static Ref Create(Type type, string methodName)
    {
      var method = type.GetMethod(methodName);
      var name = $"{type.FullName}.{method.Name}";
      return Ref.Create("Paper", name);
    }

    public static Ref Create(MethodInfo method)
    {
      var type = method.DeclaringType;
      var name = $"{type.FullName}.{method.Name}";
      return Ref.Create("Paper", name);
    }

    public string Type { get; set; }

    public string Name { get; set; }

    public StringMap Args { get; set; }

    public override string ToString()
    {
      var type = string.IsNullOrEmpty(Type) ? "" : $"{Type}/";
      var name = string.IsNullOrEmpty(Name) ? "*" : Name;
      var args = Args?.Select(arg => $"{arg.Key}={arg.Value}");
      var keys = args?.Any() == true ? $"({string.Join(";", args)})" : "";
      return $"{type}{name}{keys}";
    }

    public static Ref Parse(string path)
    {
      try
      {
        if (path == null) return null;

        var tokens = path.Split('(', ')').NotNullOrEmpty();
        var typePart = tokens.First();
        var argsPart = tokens.Skip(1).FirstOrDefault() ?? "";

        var parts = typePart.Split('/').NotNullOrEmpty().ToArray();
        var type = parts.Length > 1 ? parts.First() : null;
        var name = parts.Length > 1 ? string.Join("/", parts.Skip(1)) : typePart;

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
          Type = type,
          Name = name,
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

    public static implicit operator string(Ref @ref) => @ref?.ToString();
    public static implicit operator Ref(string @ref) => Ref.Parse(@ref);
  }
}
