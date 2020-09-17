using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Paper.Helpers;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Keep.Tools.Xml;
using Keep.Tools.Collections;
using System.Xml.Linq;
using System.Linq;
using Keep.Tools.Reflection;
using System.Collections;
using System.Xml.Serialization;
using Keep.Paper.Templating;

namespace Director
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        var file = "sample.xml";
        var text = File.ReadAllText(file);
        var xml = text.ToXElement();

        var errors = new List<string>();
        var parser = new TemplateParser();
        parser.ParseError += (o, e) => errors.Add(e.Message);

        var template = parser.ParseTemplate(xml);

        Debug.WriteLine(template.ToXElement());
        Debug.WriteLine(string.Join("\n", errors.Select(x => $"- {x}")));

        var processor = new TemplateProcessor();
        var pages = processor.ParseActions(template);

        Debug.WriteLine(pages.Count);
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
