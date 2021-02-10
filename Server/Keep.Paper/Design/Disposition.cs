using System;
using System.Collections.Generic;
using Keep.Paper.Design.Spec;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  [BaseType]
  public abstract class Disposition : Entity<Disposition>
  {
    [JsonProperty(Order = 1000)]
    public string Name => GetType().Name;

    public class Card : Disposition
    {
    }

    public class Edit : Disposition
    {
    }

    public class Grid : Disposition
    {
    }

    public class List : Disposition
    {
    }
  }
}
