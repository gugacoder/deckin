﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Tools.Web
{
  public static class HeaderExtensions
  {
    public static string GetName(this Header header)
    {
      var name = header.ToString();
      var property = typeof(HeaderNames).GetProperty(name);
      return property?.GetValue(null)?.ToString();
    }
  }
}