using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Api.Types
{
  public abstract class Template
  {
    public string Catalog { get; set; }

    public string Name { get; set; }

    public string Title { get; set; }

    public bool Disabled { get; set; }

    public Design Design { get; set; }

    public bool IgnoreUndeclaredFields { get; set; }

    public Collection<Field> Fields { get; set; }

    public Collection<Field> Filter { get; set; }
  }
}
