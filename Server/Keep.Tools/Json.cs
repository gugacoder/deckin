using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Keep.Tools
{
  public static class Json
  {
    private static readonly string[][] Escapes =
      {
        new[] { "\\"  , @"\\"  },   //  \\ Backslash character
        new[] { "\x08", @"\b"  },   //  \b Backspace (ascii code 08)
        new[] { "\x0C", @"\f"  },   //  \f Form feed (ascii code 0C)
        new[] { "\n"  , @"\n"  },   //  \n Create line
        new[] { "\r"  , @"\r"  },   //  \r Carriage return
        new[] { "\t"  , @"\t"  },   //  \t Tab
        new[] { "\""  , @"\""" }    //  \" Double quote
      };

    public static string Beautify(string json, int indentation = 2)
    {
      return string.Join("", Indent(Dedent(json), indentation));
    }

    public static string Uglify(string json)
    {
      return string.Join("", Dedent(json));
    }

    private static IEnumerable<char> Dedent(IEnumerable<char> json)
    {
      var literal = false;
      var delimiter = default(char);

      var enumerator = json.GetEnumerator();
      while (enumerator.MoveNext())
      {
        var c = enumerator.Current;
        switch (c)
        {

          case '\\':
            {
              yield return c;
              enumerator.MoveNext();
              yield return enumerator.Current;
              break;
            }

          case '\'':
          case '"':
            {
              if (literal)
              {
                if (c == delimiter)
                {
                  literal = false;
                  delimiter = default(char);
                }
              }
              else
              {
                literal = true;
                delimiter = c;
              }
              yield return c;
              break;
            }

          case ' ':
            {
              if (literal)
              {
                yield return c;
              }
              break;
            }

          default:
            {
              yield return c;
              break;
            }
        }
      }
    }

    private static IEnumerable<char> Indent(IEnumerable<char> json, int indentation)
    {
      var literal = false;
      var delimiter = default(char);

      var depth = 0;
      var enumerator = json.GetEnumerator();
      while (enumerator.MoveNext())
      {
        var c = enumerator.Current;
        switch (c)
        {

          case '\\':
            {
              yield return c;
              enumerator.MoveNext();
              yield return enumerator.Current;
              break;
            }

          case '\'':
          case '"':
            {
              if (literal)
              {
                if (c == delimiter)
                {
                  literal = false;
                  delimiter = default(char);
                }
              }
              else
              {
                literal = true;
                delimiter = c;
              }
              yield return c;
              break;
            }

          case '[':
          case '{':
            {
              yield return c;
              if (!literal)
              {
                depth++;
                yield return '\n';
                foreach (var i in Tabulate(depth, indentation)) yield return i;
              }
              break;
            }

          case ']':
          case '}':
            {
              if (!literal)
              {
                depth--;
                yield return '\n';
                foreach (var i in Tabulate(depth, indentation)) yield return i;
              }
              yield return c;
              break;
            }

          case ':':
            {
              yield return c;
              if (!literal)
              {
                yield return ' ';
              }
              break;
            }

          case ',':
            {
              yield return c;
              if (!literal)
              {
                yield return '\n';
                foreach (var i in Tabulate(depth, indentation)) yield return i;
              }
              break;
            }

          default:
            {
              yield return c;
              break;
            }
        }
      }
    }

    private static IEnumerable<char> Tabulate(int depth, int indentation)
    {
      return Enumerable.Range(0, depth * indentation).Select(x => ' ');
    }

    public static string Escape(string text)
    {
      foreach (var escape in Escapes)
      {
        text = text.Replace(escape[0], escape[1]);
      }
      return text;
    }

    public static string Unescape(string text)
    {
      foreach (var escape in Escapes)
      {
        text = text.Replace(escape[1], escape[0]);
      }
      return text;
    }

    public static string ToJson(object @object, int indentation = default)
    {
      var settings = new JsonSerializerSettings
      {
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
      };

      using var textWriter = new StringWriter();
      using var jsonWriter = new JsonTextWriter(textWriter)
      {
        Formatting = indentation > 0 ? Formatting.Indented : Formatting.None,
        Indentation = indentation
      };

      var serializer = JsonSerializer.Create(settings);
      serializer.Serialize(jsonWriter, @object);

      jsonWriter.Flush();
      textWriter.Flush();

      var json = textWriter.ToString();
      return json;
    }
  }
}