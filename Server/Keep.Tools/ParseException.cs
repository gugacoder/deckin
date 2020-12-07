using System;
namespace Keep.Tools
{
  public class ParseException : Exception
  {
    public ParseException()
    {
    }

    public ParseException(string message)
      : base(message)
    {
    }

    public ParseException(Exception cause)
      : base(cause.Message, cause)
    {
    }

    public ParseException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}
