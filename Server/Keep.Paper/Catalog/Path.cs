using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Paper.Catalog
{
  public class Path : IPath
  {
    private string _path;
    private string _name;
    private string[] _keys;

    public string Name
    {
      get => _name;
      set
      {
        _path = null;
        _name = value;
      }
    }

    public ICollection<string> Keys
    {
      get => _keys ??= new string[0];
      set
      {
        _path = null;
        _keys = value?.ToArray();
      }
    }

    public string Value => ToString();

    public override string ToString()
      => _path ??= (Keys?.Any() == true)
        ? $"{Name}({string.Join(";", Keys)})" : Name;

    public string ToString(HashMap<string> args)
    {
      var path = new StringBuilder(Name);
      if (args?.Any(x => !string.IsNullOrEmpty(x.Value)) == true)
      {
        var keys = Keys ?? new string[0];

        var invalidKey = args.Keys.FirstOrDefault(key => !keys.Contains(key));
        if (invalidKey != null)
          throw new Exception($"O argumento não é reconhecido: {invalidKey}");

        path.Append("(");

        var indexed = true;
        var count = 0;
        foreach (var key in keys)
        {
          var value = args[key];

          if (string.IsNullOrEmpty(value))
          {
            indexed = false;
            continue;
          }

          if (++count > 1)
          {
            path.Append(";");
          }

          if (!indexed || value.Contains("="))
          {
            path.Append(key).Append("=");
          }

          path.Append(value);
        }

        path.Append(")");
      }
      return path.ToString();
    }

    public IPathArgs ParseArgs(string path)
    {
      var args = new PathArgs();
      var keyNames = Keys ?? new string[0];

      // Inicializando com as chaves mas sem valor inicial
      args.AddMany(keyNames
        .Except("*")
        .Select(key => KeyValuePair.Create(key, (string)null)));

      var tokens = path.Trim().Replace(")", "").Split('(');
      var namePart = tokens.First();
      var keysPart = tokens.Skip(1).FirstOrDefault();

      if (!string.IsNullOrEmpty(namePart) && namePart != Name)
        throw new Exception(
          $"O caminho não é válido. " +
          $"Caminho esperado: `{Name}`, caminho obtido: `{namePart}`.");

      if (string.IsNullOrEmpty(keysPart))
        return args;

      var keys = (
        from keyPart in (keysPart?.Split(';') ?? Enumerable.Empty<string>())
        let parts = keyPart.Split('=')
        let key = parts.Length > 1
          ? parts.First()
          : null
        let value = parts.Length > 1
          ? string.Join("=", parts.Skip(1))
          : parts.First()
        select new { key, value }
      ).ToArray();

      var availableKeys = keyNames.Except("*").ToCollection();

      var index = 0;
      foreach (var entry in keys)
      {
        var key = entry.key;
        var value = entry.value;

        if (string.IsNullOrEmpty(key))
        {
          if (availableKeys.Count > 0)
          {
            key = availableKeys.RemoveFirst();
          }
          else if (keyNames.Contains("*"))
          {
            key = $"key{++index}";
          }
          else
          {
            throw new Exception($"Argumento não esperado: {value}");
          }
        }
        else if (availableKeys.Contains(key))
        {
          availableKeys.Remove(key);
        }
        else if (!keyNames.Contains("*"))
        {
          throw new Exception($"Argumento não esperado: {key}={value}");
        }

        args[key] = value;
      }

      return args;
    }

    public static IPathArgs ParseArgs(string pathPattern, string path)
    {
      return Parse(pathPattern).ParseArgs(path);
    }

    public static string GetName(string path) => path.Split('(').First();

    /// <summary>
    /// Realiza o parsing do padrão de nome de caminho.
    /// O padrão teve ter a forma geral:
    /// <code>
    ///   Nome.Nome...(NomeArg;NomeArg;...)
    /// </code>
    /// Exemplos:
    /// <code>
    ///   // Rota sem argumentos
    ///   MeuApp.Usuarios
    ///   // Rota com argumentos
    ///   MeuApp.Usuarios(id;login)
    /// </code>
    /// </summary>
    /// <param name="pathPattern"></param>
    /// <returns></returns>
    public static Path Parse(string pathPattern)
    {
      var tokens = pathPattern.Trim().Replace(")", "").Split('(');
      var namePart = tokens.First().Trim();
      var keysPart = tokens.Skip(1).FirstOrDefault();

      var keyNames = keysPart?.Split(';').Select(x => x.Trim()).ToArray();

      // Validando os nomes de parâmetros
      var regex = new Regex("^([*]|[a-zA-Z_][a-zA-Z_0-9]*)$");
      var invalidKeyName = keyNames?.FirstOrDefault(key => !regex.IsMatch(key));
      if (invalidKeyName != null)
        throw new Exception($"O nome do parâmetros não é válido: {invalidKeyName}");

      return new Path
      {
        Name = namePart,
        Keys = keyNames
      };
    }
  }
}
