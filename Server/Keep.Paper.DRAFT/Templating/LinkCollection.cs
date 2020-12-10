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
  public class LinkCollection : NodeCollection<Link>
  {
  }
}
