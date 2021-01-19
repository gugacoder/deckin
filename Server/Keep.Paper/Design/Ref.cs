using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [JsonConverter(typeof(RefConverter))]
  public class Ref
  {
    public Ref()
    {
    }

    public Ref(string type, string name)
    {
      this.Type = type;
      this.Name = name;
      this.Args = new StringMap();
    }

    public Ref(string type, string name, object args)
    {
      this.Type = type;
      this.Name = name;

      var entries =
        from entry in args._Map()
        select KeyValuePair.Create(entry.Key, Change.To<string>(entry.Value));

      this.Args = new StringMap(entries);
    }

    public Ref(string type, string name, StringMap args)
    {
      this.Type = type;
      this.Name = name;
      this.Args = args;
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

    public static Ref Parse(string @ref)
    {
      try
      {
        if (@ref == null) return null;

        var tokens = @ref.Split('(', ')');
        var typePart = tokens.First();
        var argsPart = tokens.Skip(1).FirstOrDefault();

        var parts = typePart.Split('/').NotNullOrEmpty().ToArray();
        var type = parts.Length > 1 ? parts.First() : null;
        var name = parts.Length > 1 ? string.Join("/", parts.Skip(1)) : typePart;

        var args = new StringMap(
          from arg in argsPart.Split(';').EmptyIfNull()
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
          $"O caminho é inválido para identificar uma ação: {@ref}", ex);
      }
    }

    public static implicit operator string(Ref @ref) => @ref?.ToString();
    public static implicit operator Ref(string @ref) => Ref.Parse(@ref);
  }
}
