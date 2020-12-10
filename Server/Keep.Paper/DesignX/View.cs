using System;
namespace Keep.Paper.DesignX
{
  public abstract class View
  {
    private string _name;

    public string Name => _name ??= GetType().Name;

    public class Grid : View
    {
    }

    public class Card : View
    {
    }

    public class Edit : View
    {
    }
  }
}
