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

namespace Keep.Paper.Templating
{
  [XmlInclude(typeof(CardAction))]
  [XmlInclude(typeof(FilterAction))]
  [XmlInclude(typeof(GridAction))]
  [XmlInclude(typeof(IntField))]
  [XmlInclude(typeof(TextField))]
  [XmlInclude(typeof(LinkTo))]
  [XmlInclude(typeof(LinkFrom))]
  public class Template : Node
  {
    private string _name;
    private string _title;
    private string _verb;

    internal string AssemblyName { get; set; }

    public string Catalog { get; set; }

    public string Name
    {
      get => _name ?? _verb?.ToPascalCase() ?? _title?.ToPascalCase();
      set => _name = value;
    }

    public string Title
    {
      get => _title ?? _verb ?? _name?.ToProperCase();
      set => _title = value;
    }

    public string Verb
    {
      get => _verb ?? _title ?? _name?.ToProperCase();
      set => _verb = value;
    }

    public bool Disabled { get; set; }
  }
}
