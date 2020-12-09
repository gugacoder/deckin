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
  public class CardAction : Action
  {
    private Query _query;

    public Query Query
    {
      get => _query;
      set => _query = Adopt(value);
    }
  }
}
