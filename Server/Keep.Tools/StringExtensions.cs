using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Keep.Tools.Collections;

namespace Keep.Tools
{
  public static class StringExtensions
  {
    #region Conversions

    public static string RemoveDiacritics(this string text)
    {
      if (!string.IsNullOrWhiteSpace(text))
      {
        string stFormD = text.Normalize(NormalizationForm.FormD);
        int len = stFormD.Length;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < len; i++)
        {
          System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
          if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
          {
            sb.Append(stFormD[i]);
          }
        }

        return (sb.ToString().Normalize(NormalizationForm.FormC));
      }
      else
      {
        return text;
      }
    }

    public static T To<T>(this string text)
    {
      return Change.To<T>(text);
    }

    public static T ToOrDefault<T>(this string text, T defaultValue = default(T))
    {
      try
      {
        return Change.To<T>(text);
      }
      catch
      {
        return defaultValue;
      }
    }

    public static MemoryStream ToStream(this string text)
    {
      var stream = new MemoryStream();
      using (var writer = new StreamWriter(stream, Encoding.UTF8, 512, true))
      {
        writer.Write(text);
        writer.Flush();
      }
      stream.Position = 0;
      return stream;
    }

    #endregion

    #region Name Conventions

    public static string ToCamelCase(this string sentence)
        => CaseChanger.ChangeCase(sentence, TextCase.CamelCase);

    public static string ToPascalCase(this string sentence)
        => CaseChanger.ChangeCase(sentence, TextCase.PascalCase);

    public static string ToProperCase(this string sentence)
        => CaseChanger.ChangeCase(sentence, TextCase.ProperCase);

    public static string ChangeCase(this string sentence, TextCase textCase)
        => CaseChanger.ChangeCase(sentence, textCase);

    #endregion

    #region Operations

    public static string Replicate(this char character, int times)
    {
      return new string(character, times);
    }

    public static string Replicate(this string text, int times)
    {
      return string.Concat(Enumerable.Range(0, times).Select(x => text));
    }

    public static string[] Split(this string text, string token)
    {
      var split = text.Split(new[] { token }, StringSplitOptions.RemoveEmptyEntries);
      return split;
    }

    public static string[] Split(this string text, params string[] tokens)
    {
      var split = text.Split(tokens, StringSplitOptions.RemoveEmptyEntries);
      return split;
    }

    /// <summary>
    /// Remove uma parte do texto correspondente à expressào regular.
    /// Mais detalhes em:
    ///   https://msdn.microsoft.com/pt-br/library/ewy2t5e0(v=vs.110).aspx
    /// </summary>
    /// <param name="text">O texto a ser pesquisado./param>
    /// <param name="regex">A expressão regular.</param>
    /// <param name="options">Opções de aplicação da expressão regular.</param>
    /// <returns>O trecho extraído da string.</returns>
    public static string Remove(this string text, string regex, RegexOptions options = default)
    {
      options |= RegexOptions.Multiline;
      text = text.Replace("\r", "");
      var replacement = "";
      return Regex.Replace(text, regex, replacement, options);
    }

    /// <summary>
    /// Extrai um trecho de uma sentença pela aplicação de uma expressão regular.
    /// 
    /// Suporta todas as substituições de Regex.Replace:
    /// -   $número     Grupo pelo índice.
    /// -   ${name}     Grupo pelo nome.
    /// -   $$          Literal "$".
    /// -   $&          Texto capturado inteiro.
    /// -   $`          Texto antes da captura.
    /// -   $'          Texto depois da captura.
    /// -   $+          Último grupo capturado.
    /// 
    /// Mais detalhes em:
    ///   https://msdn.microsoft.com/pt-br/library/ewy2t5e0(v=vs.110).aspx
    /// </summary>
    /// <param name="text">O texto a ser pesquisado./param>
    /// <param name="regex">A expressão regular.</param>
    /// <param name="replacement">
    /// Uma substituição opcional. De acordo com as regras de Regex.Replace.
    /// </param>
    /// <param name="options">Opções de aplicação da expressão regular.</param>
    /// <returns>O trecho extraído da string.</returns>
    public static string Extract(this string text, string regex, string replacement = null, RegexOptions options = default)
    {
      options |= RegexOptions.Multiline;

      text = text.Replace("\r", "");

      var match = Regex.Match(text, regex, options);
      if (!match.Success)
        return null;

      text = match.Captures[0].Value;

      if (replacement == null) replacement = "$&";
      return Regex.Replace(text, regex, replacement, options);
    }

    /// <summary>
    /// Remove uma parte do texto correspondente à expressào regular.
    /// 
    /// Suporta todas as substituições de Regex.Replace:
    /// -   $número     Grupo pelo índice.
    /// -   ${name}     Grupo pelo nome.
    /// -   $$          Literal "$".
    /// -   $&          Texto capturado inteiro.
    /// -   $`          Texto antes da captura.
    /// -   $'          Texto depois da captura.
    /// -   $+          Último grupo capturado.
    /// 
    /// Mais detalhes em:
    ///   https://msdn.microsoft.com/pt-br/library/ewy2t5e0(v=vs.110).aspx
    /// </summary>
    /// <param name="text">O texto a ser pesquisado./param>
    /// <param name="regex">A expressão regular.</param>
    /// <param name="replacement">
    /// Uma substituição opcional. De acordo com as regras de Regex.Replace.
    /// </param>
    /// <param name="options">Opções de aplicação da expressão regular.</param>
    /// <returns>O trecho extraído da string.</returns>
    public static string Inject(this string text, string regex, string replacement, RegexOptions options = default)
    {
      options |= RegexOptions.Multiline;
      text = text.Replace("\r", "");
      if (replacement == null) replacement = "$&";
      return Regex.Replace(text, regex, replacement, options);
    }

    /// <summary>
    /// Substitui um invervalo de caracteres pelo texto indicado.
    /// </summary>
    /// <param name="text">O texto a ser modificado.</param>
    /// <param name="index">A posição inicial para mudança.</param>
    /// <param name="length">A quantidade de caracteres no invervalo.</param>
    /// <param name="replacement">O texto que será posto no lugar do invervalo.</param>
    /// <returns>O texto modificado.</returns>
    public static string Inject(this string text, int index, int length, string replacement)
    {
      text = text.Substring(0, index) + replacement + text.Substring(index + length);
      return text;
    }

    public static string NullIfEmpty(this string text)
    {
      return string.IsNullOrEmpty(text) ? null : text;
    }

    public static string NullIfWhiteSpace(this string text)
    {
      return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    #endregion

    #region Comparisons

    /// <summary>
    /// Implementação de comparação similar ao comando LIKE do SQL.
    /// Os caracteres tem significado especial no padrão comparado:
    /// - "%", qualquer quantidade de caracteres na posição.
    /// - "_", um único caractere qualquer na posição.
    /// </summary>
    /// <param name="text">O texto comparado.</param>
    /// <param name="pattern">O padrão pesquisado.</param>
    /// <returns>Verdadeiro se o padrão corresponde ao texto, falso caso contrário.</returns>
    public static bool Like(this string text, string pattern)
    {
      pattern = $"^{Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".")}$";
      return Regex.IsMatch(text, pattern);
    }

    public static bool LikeIgnoreCase(this string text, string pattern)
    {
      pattern = $"^{Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".")}$";
      return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
    }

    public static bool EqualsAny(this string text, params string[] terms)
    {
      return terms.Contains(text);
    }

    public static bool EqualsAny(this string text, IEnumerable<string> terms)
    {
      return terms.Contains(text);
    }

    public static bool EqualsAnyIgnoreCase(this string text, params string[] terms)
    {
      return terms.Any(x => x.EqualsIgnoreCase(text));
    }

    public static bool EqualsAnyIgnoreCase(this string text, IEnumerable<string> terms)
    {
      return terms.Any(x => x.EqualsIgnoreCase(text));
    }

    public static bool EqualsIgnoreCase(this string text, string other)
    {
      if (text == null)
        return false;

      if (other == null)
        return false;

      return text.Equals(other, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool ContainsIgnoreCase(this string text, string pattern)
    {
      if (text == null)
        return false;

      var invariant = CultureInfo.InvariantCulture;
      var options = CompareOptions.IgnoreCase;
      return invariant.CompareInfo.IndexOf(text, pattern, options) >= 0;
    }

    public static bool ContainsAny(this string text, params string[] patterns)
    {
      if (text == null)
        return false;

      return patterns.Any(pattern => text.Contains(pattern));
    }

    public static bool ContainsAnyIgnoreCase(this string text, params string[] patterns)
    {
      if (text == null)
        return false;

      var invariant = CultureInfo.InvariantCulture;
      var options = CompareOptions.IgnoreCase;

      return patterns.Any(pattern =>
        invariant.CompareInfo.IndexOf(text, pattern, options) >= 0
      );
    }

    public static int IndexOfIgnoreCase(this string text, string pattern)
    {
      if (text == null)
        return -1;

      var invariant = CultureInfo.InvariantCulture;
      return invariant.CompareInfo.IndexOf(text, pattern);
    }

    public static bool StartsWithIgnoreCase(this string text, string term)
    {
      if (text == null)
        return false;

      var flag = StringComparison.InvariantCultureIgnoreCase;
      return text.StartsWith(term, flag);
    }

    public static bool StartsWithAny(this string text, params string[] terms)
    {
      if (text == null)
        return false;

      return terms.Any(term => text.StartsWith(term));
    }

    public static bool StartsWithAnyIgnoreCase(this string text, params string[] terms)
    {
      if (text == null)
        return false;

      var flag = StringComparison.InvariantCultureIgnoreCase;
      return terms.Any(term => text.StartsWith(term, flag));
    }

    public static bool EndsWithIgnoreCase(this string text, string term)
    {
      if (text == null)
        return false;

      var flag = StringComparison.InvariantCultureIgnoreCase;
      return text.EndsWith(term, flag);
    }

    public static bool EndsWithAny(this string text, params string[] terms)
    {
      if (text == null)
        return false;

      return terms.Any(term => text.EndsWith(term));
    }

    public static bool EndsWithAnyIgnoreCase(this string text, params string[] terms)
    {
      if (text == null)
        return false;

      var flag = StringComparison.InvariantCultureIgnoreCase;
      return terms.Any(term => text.EndsWith(term, flag));
    }

    public static bool IsNullOrEmpty(this string text)
    {
      return string.IsNullOrEmpty(text);
    }

    public static bool IsNullOrWhiteSpace(this string text)
    {
      return string.IsNullOrWhiteSpace(text);
    }

    #endregion
  }
}
