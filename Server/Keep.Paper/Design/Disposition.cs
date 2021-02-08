using System;
using System.Collections.Generic;
using Keep.Paper.Design.Spec;

namespace Keep.Paper.Design
{
  [BaseType]
  public abstract class Disposition : Entity<Disposition>
  {
    public string Name => GetType().Name;

    protected override IEnumerable<IEntity> Children() { yield break; }

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
