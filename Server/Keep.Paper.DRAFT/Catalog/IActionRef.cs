using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Keep.Tools;
using Keep.Tools.Collections;

namespace Keep.Hosting.Catalog
{
  public interface IActionRef
  {
    string Name { get; }

    ICollection<string> Keys { get; }

    string Value { get; }

    string ToString();

    string ToString(HashMap<string> args);

    IActionRefArgs ParseArgs(string path);
  }
}
