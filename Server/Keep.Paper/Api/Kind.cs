using System;
namespace Keep.Paper.Api
{
  public static class Kind
  {
    [Obsolete("Replaced by `view`.")]
    public const string Paper = "paper";

    /// <summary>
    /// Entidade de transporte de informação trivial.
    /// </summary>
    public const string Info = "info";

    /// <summary>
    /// Entidade de transporte de informação sobre falha.
    /// </summary>
    public const string Fault = "fault";

    /// <summary>
    /// Entidade de transporte de especificação de ação.
    /// </summary>
    public const string Action = "action";

    /// <summary>
    /// Entidade de transporte de especificação de campo.
    /// </summary>
    public const string Field = "field";

    /// <summary>
    /// A entidade representa um dado qualquer.
    /// </summary>
    public const string Data = "data";

    /// <summary>
    /// Entidade de transporte de validação de dados e formulários.
    /// </summary>
    public const string Validation = "validation";
  }
}
