using System;
namespace Keep.Paper.Design
{
  public class FieldFace
  {
    private FieldFace()
    {
    }

    public string Name => GetType().Name;

    public class Label : FieldFace
    {
    }

    public class Input : FieldFace
    {
    }
  }
}
