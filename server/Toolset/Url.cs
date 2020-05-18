using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Toolset.Collections;

namespace Toolset
{
  public class Url : ICloneable
  {
    private static readonly Regex namePattern = new Regex(@"^[^:/@]*$");
    private static readonly Regex pathPattern = new Regex(@"^(/[^/?]+)+/?$");

    private static readonly Regex datePattern = new Regex(@"^(\d{2}:\d{2}(:\d{2}(\.\d{3})?([+-]\d{1,2}:\d{2})?)?|^\d{4}-\d{2}-\d{2}(T\d{2}:\d{2}(:\d{2}(\.\d{3})?([+-]\d{1,2}:\d{2})?)?)?)$");
    private static readonly Regex numberPattern = new Regex(@"^-?[01]?\d{0,9}$");
    private static readonly Regex decimalPattern = new Regex(@"^-?\d+[.,]\d+$");

    private static Regex uriPattern;

    private string _protocol;
    private string _host;
    private string _user;
    private string _pass;
    private string _path;

    public Url()
    {
    }

    public Url(string url)
    {
      Replace(url);
    }

    public string Protocol
    {
      get => string.IsNullOrEmpty(_protocol) ? null : _protocol;
      set
      {
        if (!string.IsNullOrEmpty(value) && !namePattern.IsMatch(value))
          throw new Exception($"O texto informado não corresponde a um protocolo válido: {value}");

        _protocol = value;
      }
    }

    public string Host
    {
      get => string.IsNullOrEmpty(_host) ? null : _host;
      set
      {
        if (!string.IsNullOrEmpty(value) && !namePattern.IsMatch(value))
          throw new Exception($"O texto informado não corresponde a um host ou IP válido: {value}");

        _host = value;
      }
    }

    public string User
    {
      get => string.IsNullOrEmpty(_user) ? null : _user;
      set
      {
        if (!string.IsNullOrEmpty(value) && !namePattern.IsMatch(value))
          throw new Exception($"O texto informado não corresponde a um nome de usuário válido: {value}");

        _user = value;
      }
    }

    public string Pass
    {
      get => string.IsNullOrEmpty(_pass) ? null : _pass;
      set
      {
        if (!string.IsNullOrEmpty(value) && !namePattern.IsMatch(value))
          throw new Exception($"O texto informado não corresponde a uma senha válida: {value}");

        _pass = value;
      }
    }

    public int? Port { get; set; }

    public string Path
    {
      get => string.IsNullOrEmpty(_path) ? null : _path;
      set
      {
        if (!string.IsNullOrEmpty(value) && !pathPattern.IsMatch(value))
          throw new Exception($"O texto informado não corresponde a um caminho válido: {value}");

        _path = value;
      }
    }

    public IMap<string, Var> Args { get; } = new HashMap<Var>();

    public Url Clone()
    {
      var url = new Url();
      url.Protocol = this.Protocol;
      url.User = this.User;
      url.Pass = this.Pass;
      url.Host = this.Host;
      url.Port = this.Port;
      url.Path = this.Path;

      var queryString = StringifyArgs(this.Args);
      url.Replace($"?{queryString}");

      return url;
    }

    object ICloneable.Clone()
    {
      return Clone();
    }

    public Url Append(params string[] fragments)
    {
      var allTokens = fragments.Select(TokenizeUrl).ToArray();
      SetBase(allTokens);
      SetPath(allTokens);
      SetArgs(allTokens);
      return this;
    }

    public Url Combine(params string[] fragments)
    {
      var allTokens = fragments.Select(TokenizeUrl).ToArray();
      SetBase(allTokens);
      SetPath(allTokens);
      SetArgs(allTokens);
      return this;
    }

    public Url Replace(params string[] fragments)
    {
      var allTokens = fragments.Select(TokenizeUrl).ToArray();
      SetBase(allTokens);
      SetPath(allTokens);
      SetArgs(allTokens);
      return this;
    }

    private void SetBase(Tokens[] allTokens, [CallerMemberName] string method = null)
    {
      var protocol = allTokens.Select(x => x.Protocol).NotNullOrEmpty().LastOrDefault();
      var user = allTokens.Select(x => x.User).NotNullOrEmpty().LastOrDefault();
      var pass = allTokens.Select(x => x.Pass).NotNullOrEmpty().LastOrDefault();
      var host = allTokens.Select(x => x.Host).NotNullOrEmpty().LastOrDefault();
      var port = allTokens.Select(x => x.Port).NotNullOrEmpty().LastOrDefault();

      int? portNumber = null;
      if (int.TryParse(port, out int number)) portNumber = number;

      if (method == nameof(Replace))
      {
        if (!string.IsNullOrEmpty(protocol)) Protocol = protocol;
        if (!string.IsNullOrEmpty(host))
        {
          User = user;
          Pass = pass;
          Host = host;
          Port = portNumber;
        }
        else
        {
          if (!string.IsNullOrEmpty(user)) User = user;
          if (!string.IsNullOrEmpty(pass)) Pass = pass;
          if (!string.IsNullOrEmpty(port)) Port = portNumber;
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(protocol)) Protocol = protocol;
        if (!string.IsNullOrEmpty(user)) User = user;
        if (!string.IsNullOrEmpty(pass)) Pass = pass;
        if (!string.IsNullOrEmpty(host)) Host = host;
        if (!string.IsNullOrEmpty(port)) Port = portNumber;
      }
    }

    private void SetPath(Tokens[] allTokens, [CallerMemberName] string method = null)
    {
      var paths = allTokens.Select(x => x.Path).NotNullOrEmpty();
      if (!paths.Any())
        return;

      var oldNodes = (
        from node in (this?.Path ?? "").Split('/')
        select node
      ).NotNullOrEmpty().ToArray();

      var newNodes = (
        from item in paths.NotNullOrEmpty()
        from node in item.Split('/')
        select node
      ).NotNullOrEmpty().ToArray();

      var upwards = newNodes.TakeWhile(x => x == ".." || x == ".").ToArray();
      var upwardCount = upwards.Where(x => x == "..").Count();
      newNodes = newNodes.Skip(upwards.Length).ToArray();

      string path = null;

      switch (method)
      {
        case nameof(Replace):
          {
            path = "/" + string.Join("/", newNodes);
            break;
          }
        case nameof(Append):
          {
            if (upwardCount > 0)
            {
              oldNodes = oldNodes.SkipLast(upwardCount).ToArray();
            }
            path = "/" + string.Join("/", oldNodes.Concat(newNodes));
            break;
          }
        case nameof(Combine):
          {
            var initialChar = allTokens
              .Select(x => x.Path)
              .NotNullOrEmpty()
              .FirstOrDefault()
              ?.FirstOrDefault();

            if (initialChar == '/')
            {
              path = "/" + string.Join("/", newNodes);
            }
            else
            {
              if (this.Path.EndsWith("/"))
              {
                path = "/" + string.Join("/",
                  oldNodes
                    .SkipLast(upwardCount)
                    .Concat(newNodes)
                  );
              }
              else
              {
                path = "/" + string.Join("/",
                  oldNodes
                    .Take(oldNodes.Length - 1)
                    .SkipLast(upwardCount)
                    .Concat(newNodes)
                  );
              }
            }
            break;
          }
      }

      var terminalChar = allTokens
        .Select(x => x.Path)
        .NotNullOrEmpty()
        .FirstOrDefault()
        ?.FirstOrDefault();

      if (terminalChar == '/')
      {
        path += "/";
      }

      this.Path = path;
    }

    private void SetArgs(Tokens[] allTokens, [CallerMemberName] string method = null)
    {
      var args = (
        from terms in allTokens.Select(x => x.Args).NotNullOrEmpty()
        from pair in terms.Split('&').NotNullOrEmpty()
        let tokens = pair.Split('=')
        let key = tokens.First()
        let value = (tokens.Length == 1) ? "1" : string.Join("=", tokens.Skip(1))
        select new { key, value }
      ).ToArray();

      var map = new HashMap<Var>();
      foreach (var arg in args)
      {
        var name = Regex.Replace(arg.key, @"[.\[\]].*", "");
        var value = CastToClrValue(arg.value);

        var isArray = arg.key.Contains("[");
        var isRange = arg.key.Contains(".min") || arg.key.Contains(".max");

        if (isArray)
        {
          var current = map[name];
          if (current?.IsArray != true)
          {
            map[name] = current = new Var(new ArrayList());
          }
          current.Array.Add(value);
        }
        else if (isRange)
        {
          var current = map[name];
          if (current?.IsRange != true)
          {
            map[name] = current = new Var(new Toolset.Range());
          }
          if (arg.key.Contains(".min"))
          {
            current.Range.Min = value;
          }
          else
          {
            current.Range.Max = value;
          }
        }
        else
        {
          map[name] = new Var(value);
        }
      }

      if (method.Equals(nameof(Replace)))
      {
        this.Args.Clear();
      }

      foreach (var entry in map)
      {
        if (method == nameof(Combine) || method == nameof(Append))
        {
          var oldValue = Args[entry.Key];
          var newValue = entry.Value;

          if (oldValue?.IsArray == true && newValue?.IsArray == true)
          {
            newValue.Array.Cast<object>().ForEach(x => oldValue.Array.Add(x));
            continue;
          }
          if (oldValue?.IsRange == true && newValue?.IsRange == true)
          {
            if (newValue.Range.Min != null) oldValue.Range.Min = newValue.Range.Min;
            if (newValue.Range.Max != null) oldValue.Range.Max = newValue.Range.Max;
            continue;
          }
        }
        Args[entry.Key] = entry.Value;
      }
    }

    private object CastToClrValue(string value)
    {
      var isQuoted = (value.StartsWith("\"") && value.StartsWith("\""))
                  || (value.StartsWith("'") && value.StartsWith("'"));
      if (isQuoted)
        return value;

      if (datePattern.IsMatch(value)) return Change.To<DateTime>(value);
      if (numberPattern.IsMatch(value)) return Change.To<int>(value);
      if (decimalPattern.IsMatch(value)) return Change.To<decimal>(value);

      if (value.EqualsIgnoreCase("true") || value.EqualsIgnoreCase("yes") || value.EqualsIgnoreCase("on"))
        return true;
      if (value.EqualsIgnoreCase("false") || value.EqualsIgnoreCase("no") || value.EqualsIgnoreCase("off"))
        return false;

      return value;
    }

    public Url ClearArgs()
    {
      Args?.Clear();
      return this;
    }

    public Url SetArg(string argName, object value)
    {
      if (value == null)
      {
        Args.Remove(argName);
      }
      else
      {
        Args[argName] = value as Var ?? new Var(value);
      }
      return this;
    }

    public Url SetArgs(params object[] argValuePairs)
    {
      if ((argValuePairs.Length % 2) != 2)
        throw new Exception("Era esperado um vetor com pares de parâmetros. Cada índice par como um nome de parâmetro e cada índice ímpar como valor do parâmetro. Ex: [ \"id\", 1, \"nome\", \"Fulano\" ]");

      for (var i = 0; i < argValuePairs.Length; i += 2)
      {
        var key = argValuePairs[i]?.ToString();
        var value = argValuePairs[i + 1];
        SetArg(key, value);
      }

      return this;
    }

    public override string ToString()
    {
      var builder = new StringBuilder();

      var path = Path;

      if (Protocol != null || Host != null || Port != null || User != null || Pass != null)
      {
        var prot = Protocol ?? "*";
        var host = Host ?? "*";

        string port;
        string credentials = null;

        if (Port == null)
          port = "";
        else if (Port == 0)
          port = ":*";
        else
          port = $":{Port}";

        if (User != null || Pass != null)
        {
          var user = User ?? "*";
          var pass = (Pass != null) ? $":{Pass}" : "";
          credentials = $"{user}{pass}@";
        }

        builder.Append(prot).Append("://").Append(credentials).Append(host).Append(port);

        if (string.IsNullOrEmpty(path))
        {
          path = "/";
        }
      }

      builder.Append(path);

      var args = StringifyArgs(this.Args);
      if (!string.IsNullOrEmpty(args))
      {
        builder.Append("?").Append(args);
      }

      return builder.ToString();
    }

    private static string StringifyArgs(IMap<string, Var> map)
    {
      if (map?.Any() != true)
        return null;

      var separator = "";

      var builder = new StringBuilder();

      foreach (var entry in map)
      {
        var key = entry.Key;
        var var = entry.Value;

        if (var == null || var.IsNull)
          continue;

        if (var.IsValue)
        {
          builder.Append(separator).Append(key);
          if (var.Value is bool bit)
          {
            builder.Append("=").Append(bit ? "1" : "0");
          }
          else
          {
            var text = Change.To<string>(var.Value);
            builder.Append("=").Append(text);
          }
          separator = "&";
        }
        else if (var.IsRange)
        {
          if (var.Range.IsMinSet)
          {
            var text = Change.To<string>(var.Range.Min);
            builder.Append(separator).Append(key).Append(".min=").Append(text);
            separator = "&";
          }
          if (var.Range.IsMaxSet)
          {
            var text = Change.To<string>(var.Range.Max);
            builder.Append(separator).Append(key).Append(".max=").Append(text);
            separator = "&";
          }
        }
        else if (var.IsArray && var.Array.Count > 0)
        {
          foreach (var value in var.Array)
          {
            var text = Change.To<string>(value);
            builder.Append(separator).Append(key).Append("[]=").Append(text);
            separator = "&";
          }
        }
      }

      return builder.ToString();
    }

    public static implicit operator string(Url url)
    {
      return url?.ToString();
    }

    public static implicit operator Url(string url)
    {
      return new Url(url);
    }

    private static Tokens TokenizeUrl(string url)
    {
      if (uriPattern == null)
      {
        uriPattern = new Regex(@"^(?:(?:([^/@:]*):)?//(?:([^/@:]+)(?::([^/@:]+))?@)?([^/@:]+)(?::(\d+))?)?([^:?]+)?(?:\?(.*))?$");
      }

      var tokens = new Tokens();

      var match = uriPattern.Match(url);
      if (match.Success)
      {
        tokens.Protocol = match.Groups[1].Value;
        tokens.User = match.Groups[2].Value;
        tokens.Pass = match.Groups[3].Value;
        tokens.Host = match.Groups[4].Value;
        tokens.Port = match.Groups[5].Value;
        tokens.Path = match.Groups[6].Value;
        tokens.Args = match.Groups[7].Value;
      }

      return tokens;
    }

    private struct Tokens
    {
      public string Protocol { get; set; }
      public string User { get; set; }
      public string Pass { get; set; }
      public string Host { get; set; }
      public string Port { get; set; }
      public string Path { get; set; }
      public string Args { get; set; }
    }
  }
}
