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
  public class PaperTemplate : Template
  {
    private DataCollection _localData;
    private ActionCollection _actions;
    private LinkCollection _links;

    public string DefaultConnection { get; set; }
    //public string Entity { get; set; }
    //public Fields Fields { get; set; }

    public DataCollection LocalData
    {
      get => _localData;
      set => _localData = Adopt(value);
    }

    public ActionCollection Actions
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
