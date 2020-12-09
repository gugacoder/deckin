using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Keep.Hosting.Api;
using Keep.Hosting.Api.Types;
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

namespace Keep.Hosting.Templating
{
  public class Action : Node
  {
    private string _name;
    private string _title;
    private string _verb;
    private FieldCollection _fields;
    private Action _actions;
    private LinkCollection _links;

    public virtual string Rel { get; set; }

    public string Catalog { get; set; }

    public string Collection { get; set; }

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

    public bool Popup { get; set; }

    public string Extent { get; set; }

    public FieldCollection Fields
    {
      get => _fields;
      set => _fields = Adopt(value);
    }

    public Action Actions
    {
      get => _actions;
      set => _actions = Adopt(value);
    }

    public LinkCollection Links
    {
      get => _links;
      set => _links = Adopt(value);
    }
  }
}
