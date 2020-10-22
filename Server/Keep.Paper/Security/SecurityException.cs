using System;
namespace Keep.Paper.Security
{
  public class SecurityException : Exception
  {
    public SecurityException()
    {
    }

    public SecurityException(string message)
      : base(message)
    {
    }

    public SecurityException(Exception cause)
      : base(cause.Message, cause)
    {
    }

    public SecurityException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}
