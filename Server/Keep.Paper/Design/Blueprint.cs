using System;
using System.Collections.Generic;
using Keep.Paper.Design.Spec;
using Newtonsoft.Json;

namespace Keep.Paper.Design
{
  public abstract class Blueprint : Entity<Blueprint>
  {
    [BaseType]
    public class Card : Blueprint
    {
    }

    [BaseType]
    public class Edit : Blueprint
    {
    }

    [BaseType]
    public class Grid : Blueprint
    {
    }

    [BaseType]
    public class List : Blueprint
    {
    }
  }
}
