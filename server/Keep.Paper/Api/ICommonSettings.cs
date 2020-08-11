using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.Extensions.Configuration;

namespace Keep.Paper.Api
{
  public interface ICommonSettings
  {
    ICollection<string> Keys { get; }

    string CanonicalizeKey(string key);

    string Get(string key, string defaultValue = null);

    T Get<T>(string key, T defaultValue = default);

    Ret Set(string key, object value);

    void Reload();
  }
}