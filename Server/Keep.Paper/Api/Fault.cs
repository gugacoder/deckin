using System;
namespace Keep.Paper.Api
{
  public static class Fault
  {
    public const string NotFound = nameof(NotFound);
    public const string Unauthorized = nameof(Unauthorized);
    public const string Forbidden = nameof(Forbidden);
    public const string ServerFailure = nameof(ServerFailure);
  }
}
