using System;
namespace Keep.Paper.Api
{
  public static class Severities
  {
    // Abaixo seguem comentários de recomendação para esquemas de cores,
    // na forma "Fundo; Texto".

    // Transparente; Grafite
    public const string Default = nameof(Default);

    // Cinza Claro; Preto
    public const string Information = nameof(Information);

    // Verde-Água Claro; Verde-Água Escuro
    public const string Highlighted = nameof(Highlighted);

    // Azul Claro; Azul Escuro
    public const string Primary = nameof(Primary);

    // Cinza Claro; Gafite
    public const string Secondary = nameof(Secondary);

    // Verde Claro; Verde Escuro
    public const string Success = nameof(Success);

    // Amarelo Claro; Chocolate
    public const string Warning = nameof(Warning);

    // Vermelho Claro; Vermelho Escuro
    public const string Danger = nameof(Danger);
  }
}
