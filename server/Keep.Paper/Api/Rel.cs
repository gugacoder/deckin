using System;
namespace Keep.Paper.Api
{
  public static class Rel
  {
    // Marca um elemento como representando o próprio objeto.
    public const string Self = "self";

    // Marca um objeto como sendo o item de uma coleção de objetos.
    public const string Item = "item";

    // Conecta um elemento à ação básica de entrar no sistema.
    public const string Login = "login";

    // Conecta um elemento à ação básica de sair do sistema.
    public const string Logout = "logout";

    // Marca um elemento como sendo o destino de uma navegação.
    public const string Forward = "forward";

    // Marca um elemento como sendo uma ação.
    public const string Action = "action";

    // Marca um elemento como destino prioritário de uma ação.
    public const string Primary = "primary";
  }
}
