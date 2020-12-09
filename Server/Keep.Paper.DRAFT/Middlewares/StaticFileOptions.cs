using System;
using System.Collections.Generic;
using System.Text;

namespace Keep.Hosting.Middlewares
{
  public class StaticFileOptions
  {
    internal bool DefaultFiles { get; set; }

    public StaticFileOptions UseDefaultFiles()
    {
      this.DefaultFiles = true;
      return this;
    }
  }
}
