using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Keep.Tools;

namespace Keep.Paper.Templating
{
  public class Query : Node
  {
    private string _server;

    public string Name { get; set; }

    public string Connection { get; set; }

    public string Server
    {
      get
      {
        if (!string.IsNullOrEmpty(_server))
          return _server;

        if (Connection.EqualsIgnoreCase("LocalData"))
          return MakeTableName(Template);

        return null;
      }
      set => _server = value;
    }

    public int? Port { get; set; }

    public string Database { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    [XmlText]
    public string Text { get; set; }

    public static string MakeTableName(Template template)
    {
      return Regex.Replace(template?.Name ?? "", @"[^\w\d._]", "_");
    }
  }
}
