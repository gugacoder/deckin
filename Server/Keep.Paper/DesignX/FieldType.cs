using System;
namespace Keep.Paper.DesignX
{
  public abstract class FieldType
  {
    private string _name;

    public string Name => _name ??= GetType().Name;

    public class Variant : FieldType
    {
    }

    public class Boolean : FieldType
    {
    }

    public class Integer : FieldType
    {
    }

    public class Decimal : FieldType
    {
    }

    public class Char : FieldType
    {
    }

    public class Text : FieldType
    {
    }

    public class Digits : FieldType
    {
    }

    public class DateTime : FieldType
    {
    }

    public class Date : FieldType
    {
    }

    public class Time : FieldType
    {
    }

    public class Guid : FieldType
    {
    }
  }
}
