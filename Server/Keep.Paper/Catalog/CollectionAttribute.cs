using System;
using System.Text.RegularExpressions;

namespace Keep.Paper.Catalog
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class CollectionAttribute : Attribute
  {
    public CollectionAttribute(string name)
    {
      this.Name = name;
    }

    public string Name { get; }

    public void Validate()
    {
      var pattern = @"^[a-zA-Z_][a-zA-Z0-9._]*$";
      if (Name != null && !Regex.IsMatch(Name, pattern))
      {
        throw new Exception(
         $"Formato inválido para um nome de coleção: {Name} \n" +
          "Padrão esperado: \n" +
         $"-  {pattern} \n" +
          "Regras de nome: \n" +
          "-  Deve conter letras (a-z,A-Z), números (0-9) e sublinhas (_) \n" +
          "-  Deve começar por letra (a-z,A-Z) ou sublinha (_) \n" +
          "-  Palavras podem ser separadas por ponto (Palavra1.Palavra2) \n" +
          "Exemplos: \n" +
          "-  Nome.Da.Colecao \n"
        );
      }
    }
  }
}
