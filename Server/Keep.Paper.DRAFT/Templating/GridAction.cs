﻿using System;
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
  public class GridAction : Action
  {
    private Query _query;
    private FieldCollection _filter;

    public int? AutoRefresh { get; set; }

    public int? Offset { get; set; }

    public int? Limit { get; set; }

    public Query Query
    {
      get => _query;
      set => _query = Adopt(value);
    }

    public FieldCollection Filter
    {
      get => _filter;
      set => _filter = Adopt(value);
    }
  }
}
