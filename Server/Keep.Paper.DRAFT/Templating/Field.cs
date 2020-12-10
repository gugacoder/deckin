using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
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
  public class Field : Node
  {
    private OptionCollection _options;

    public virtual string Type { get; set; }

    public virtual string Rel { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public bool? Hidden { get; set; }

    public bool? ReadOnly { get; set; }

    public OptionCollection Options
    {
      get => _options;
      set => _options = Adopt(value);
    }
  }
}
