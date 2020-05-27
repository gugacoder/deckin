using System;
namespace Keep.Paper.Api
{
  public static class Kind
  {
    // System Wide Actions
    public const string Meta = "meta";

    // Notifications
    public const string Fault = "fault";
    public const string Validation = "validation";

    // Structural Elements
    public const string Field = "field";
    public const string Action = "action";
    public const string Collection = "collection";
  }
}
