using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using Keep.Tools;
using Keep.Tools.Reflection;

namespace Keep.Paper.Templating
{
  public class TemplateParser
  {
    public class ParseErrorEventArgs : EventArgs
    {
      public ParseErrorEventArgs(string text)
      {
        this.Message = text;
      }

      public string Message { get; }

      public static implicit operator ParseErrorEventArgs(string text)
      {
        return new ParseErrorEventArgs(text);
      }
    }

    public event EventHandler<ParseErrorEventArgs> ParseError;

    public Template ParseTemplate(XElement xml)
    {
      var key = xml.Name.LocalName;
      var baseType = typeof(Template);
      var typeName = $"{baseType.Namespace}.{key.ToPascalCase()}";
      var type = Type.GetType(typeName);

      var template = (Template)Activator.CreateInstance(type);

      ParseNode(xml, template);

      return template;
    }

    private void ParseNode(XElement xml, object target)
    {
      foreach (var attr in xml.Attributes())
      {
        var key = attr.Name.LocalName;
        var value = attr.Value;
        var ok = target._TrySet(key, value);
        if (!ok)
        {
          ParseError?.Invoke(this, $"A chave não é reconhecida: {key}");
        }
      }

      foreach (var tag in xml.Elements())
      {
        var key = tag.Name.LocalName;
        var keyType = target._TypeOf(key);

        if (keyType == typeof(string) || keyType?.IsValueType == true)
        {
          target._Set(key, tag.Value);
          continue;
        }

        if (keyType != null)
        {
          var content = target._SetNew(key);
          if (tag.Attributes().Any() || tag.Elements().Any())
          {
            ParseNode(tag, content);
          }
          continue;
        }

        var items = target as IList;
        if (items == null)
        {
          ParseError?.Invoke(this, $"A chave não é reconhecida: {key}");
          continue;
        }

        var baseType = typeof(Template);
        var typeName = $"{baseType.Namespace}.{key.ToPascalCase()}";
        var type = Type.GetType(typeName);
        if (type == null)
        {
          ParseError?.Invoke(this, $"A chave não é reconhecida: {key}");
          continue;
        }

        var value = Activator.CreateInstance(type);

        if (tag.Attributes().Any() || tag.Elements().Any())
        {
          ParseNode(tag, value);
        }

        items.Add(value);
      }
    }
  }
}