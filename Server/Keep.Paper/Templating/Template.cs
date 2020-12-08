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
  [XmlInclude(typeof(CardAction))]
  [XmlInclude(typeof(FilterAction))]
  [XmlInclude(typeof(GridAction))]
  [XmlInclude(typeof(IntField))]
  [XmlInclude(typeof(TextField))]
  [XmlInclude(typeof(LinkTo))]
  [XmlInclude(typeof(LinkFrom))]
  public class Template : Node
  {
    volatile private static int randomNameCountGenerator = 0;
    private int randomNameCount;

    private string _name;
    private string _assemblyName;
    private string _manifestName;

    internal string AssemblyName
    {
      get => _assemblyName;
      set
      {
        _assemblyName = value;
        _name = null;
      }
    }

    internal string ManifestName
    {
      get => _manifestName;
      set
      {
        _manifestName = value;
        _name = null;
      }
    }

    public string Name
    {
      get => _name ??= $"{AssemblyName}.{ManifestName ?? MakeRandonName()}";
      set => _name = value;
    }

    public string Collection { get; set; }

    public bool Disabled { get; set; }

    private string MakeRandonName()
    {
      this.randomNameCount = ++randomNameCountGenerator;
      return $"Template-{++randomNameCount}";
    }
  }
}
