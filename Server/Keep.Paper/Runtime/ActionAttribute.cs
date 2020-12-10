using System;
using System.Text.RegularExpressions;

namespace Keep.Paper.Runtime
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class ActionAttribute : Attribute
  {
    public ActionAttribute(string pattern)
    {
      this.Pattern = pattern;
    }

    public string Pattern { get; }

    public void Validate()
    {
      var pattern = @"^(\.?[a-zA-Z_][a-zA-Z0-9._]*|\.?[a-zA-Z_][a-zA-Z0-9._]*\([.]{3}\)|\.?[a-zA-Z_][a-zA-Z0-9._]*\((\*|[a-zA-Z_][\w\d_]*)(;(\*|[a-zA-Z_][\w\d_]*))*\))$";
      if (Pattern != null && !Regex.IsMatch(Pattern, pattern))
      {
        throw new Exception(
         $"Formato inválido para a construção de uma ação: {Pattern} \n" +
          "Padrão esperado: \n" +
         $"-  {pattern} \n" +
          "Regras de nome: \n" +
          "-  Deve conter letras (a-z,A-Z), números (0-9) e sublinhas (_) \n" +
          "-  Deve começar por letra (a-z,A-Z) ou sublinha (_) \n" +
          "-  Palavras podem ser separadas por ponto (Palavra1.Palavra2) \n" +
          "Exemplos: \n" +
          "-  Nome.Da.Acao \n" +
          "-  Nome.Da.Acao(Arg1;Arg2;*)"
        );
      }
    }
  }
}
