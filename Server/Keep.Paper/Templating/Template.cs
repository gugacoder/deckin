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
    internal string AssemblyName { get; set; }

    public string Catalog { get; set; }

    public string Collection { get; set; }

    public bool Disabled { get; set; }
  }
}
