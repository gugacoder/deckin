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
using Keep.Hosting.Templating;
using System.Diagnostics.CodeAnalysis;
using System.Data;
using System.Text.RegularExpressions;
using Keep.Hosting.Auth;

namespace Mercadologic.Replicacao
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
      }
      catch (Exception ex)
      {
        Console.WriteLine("FAIL!!!");
        Console.WriteLine(ex.GetCauseMessage());
        Console.WriteLine(ex.GetStackTrace());
      }
    }

    //public const string Anonymous = nameof(Anonymous);

    //private Regex regex = new Regex(@"^(?:([\w-._]+)?([/\\:]))?(.+)$");
    //private char? separator;

    //public string User { get; private set; }
    //public string Domain { get; private set; }

    //public string GetLoginName() => $"{Domain}{separator}{User}";

    //public void SetLoginName(string user, string domain)
    //{
    //  this.User = user;
    //  if (!string.IsNullOrEmpty(domain))
    //  {
    //    this.Domain = domain;
    //    this.separator = '/';
    //  }
    //  else
    //  {
    //    this.Domain = null;
    //    this.separator = null;
    //  }
    //}

    //public void SetLoginName(string login)
    //{
    //  var match = regex.Match(login);
    //  if (!match.Success)
    //  {
    //    this.User = login;
    //    this.Domain = null;
    //    this.separator = null;
    //    return;
    //  }

    //  var domain = match.Groups[1].Value;
    //  var separator = match.Groups[2].Value;
    //  var username = match.Groups[3].Value;

    //  if (string.IsNullOrEmpty(domain))
    //  {
    //    domain = null;
    //    separator = null;
    //  }
    //  else if (separator != ":")
    //  {
    //    separator = "/";
    //  }

    //  this.User = username;
    //  this.Domain = domain;
    //  this.separator = separator?.First();
    //}
  }
}
