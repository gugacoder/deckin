using System;
namespace Keep.Hosting.Api
{
  public static class Kind
  {
    [Obsolete("Replaced by `action`.")]
    public const string Paper = "paper";
    [Obsolete("Replaced by `action`.")]
    public const string Info = "info";
    [Obsolete("Replaced by `status`.")]
    public const string Validation = "validation";
    [Obsolete("Replaced by `status`.")]
    public const string Fault = "fault";

    /// <summary>
    /// Representação de mensagens de falha, status ou validação.
    /// </summary>
    public const string Status = "status";

    public const string Action = "action";

    public const string Field = "field";
  }
}
