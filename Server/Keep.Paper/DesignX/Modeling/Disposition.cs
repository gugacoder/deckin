using System;
namespace Keep.Paper.DesignX
{
  public abstract class Disposition
  {
    private string _name;

    public string Name => _name ??= GetType().Name;

    public class Grid : Disposition
    {
    }

    public class Card : Disposition
    {
    }

    public class Edit : Disposition
    {
    }
  }
}
