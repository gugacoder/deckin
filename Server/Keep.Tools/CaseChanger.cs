using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Keep.Tools.Collections;

namespace Keep.Tools
{
  public static class CaseChanger
  {
    public class Options
    {
      public char[] WordCharacters { get; set; }
      public char[] PreservedCharacters { get; set; }
    }

    public class Word
    {
      public bool IsSymbol { get; set; }
      public bool IsWord =>
        !IsSymbol &&
        !string.IsNullOrEmpty(Text) &&
        Text.Any(c => c != '_');

      public string Text { get; set; }
      public string Prefix { get; set; }
      public string Suffix { get; set; }

      public override string ToString() => $"{Prefix}{Text}{Suffix}";
    }

    public static readonly char[] DefaultWordCharacters =
      "`'_".ToArray();

    public static readonly char[] DefaultPreservedCharacters =
      "".ToArray();

    public static readonly char[] AllPreservedCharacters =
      @"""(){}[]<>|/\!?~@#$%^&,;:*+=".ToArray();

    public static string ChangeCase(string sentence,
      TextCase textCase, Options options = null)
    {
      var wordCharacters = options?.WordCharacters;
      var preservedCharacters = options?.PreservedCharacters;

      SelectCharacters(textCase, ref wordCharacters, ref preservedCharacters);

      textCase = SanitizeTextCase(textCase);

      var words = EnumerateWords(sentence, textCase,
        wordCharacters, preservedCharacters)
          .ToArray();

      var @case = SelectCase(textCase);
      switch (@case)
      {
        case TextCase.UpperCase:
          words.Where(w => w.IsWord).ForEach(w => w.Text = w.Text.ToUpper());
          break;
        case TextCase.LowerCase:
          words.Where(w => w.IsWord).ForEach(w => w.Text = w.Text.ToLower());
          break;
        case TextCase.ProperCase:
          words.Where(w => w.IsWord)
            .ForEach(w =>
            {
              var first = w.Text.Substring(0, 1).ToUpper();
              var others = w.Text.Substring(1).ToLower();
              w.Text = $"{first}{others}";
            });
          break;
      }

      if (textCase.HasFlag(TextCase.StartLowerCase) ||
          textCase.HasFlag(TextCase.StartUpperCase))
      {
        var word = words
          .Where(w => w.IsWord)
          .FirstOrDefault();
        if (word != null)
        {
          var first = word.Text.Substring(0, 1);
          var others = word.Text.Substring(1);

          first = textCase.HasFlag(TextCase.StartLowerCase)
            ? first.ToLower() : first.ToUpper();

          word.Text = $"{first}{others}";
        }
      }

      var result = JoinWords(textCase, words);

      if (!textCase.HasFlag(TextCase.PreserveDiacritics))
      {
        result = result.RemoveDiacritics();
      }

      if (textCase.HasFlag(TextCase.NoPrefix))
      {
        if (result.StartsWith("-"))
          result = Regex.Replace(result, "^-+", "");
        else if (result.StartsWith("_"))
          result = Regex.Replace(result, "^_+", "");
      }

      return result;
    }

    private static string JoinWords(TextCase textCase, Word[] words)
    {
      if (!words.Any())
        return string.Empty;

      var separator = SelectSeparator(textCase);
      if (separator == " ")
      {
        var tokens = words.Select(w => $"{w.Prefix}{w.Text}{w.Suffix}");
        return string.Join(separator, tokens);
      }

      var builder = new StringBuilder();
      builder.Append(words.First());

      var previous = words.First();
      foreach (var word in words.Skip(1))
      {
        if (string.IsNullOrEmpty(previous.Suffix) &&
            string.IsNullOrEmpty(word.Prefix))
        {
          // Há necessidade de separador
          builder.Append(separator);
        }

        builder.Append(word);
        previous = word;
      }

      return builder.ToString();
    }

    private static TextCase SanitizeTextCase(TextCase textCase)
    {
      var hasCase = new[] {
        TextCase.KeepOriginal,
        TextCase.UpperCase,
        TextCase.LowerCase,
        TextCase.ProperCase
      }.Any(@case => textCase.HasFlag(@case));

      if (!hasCase)
      {
        if (textCase.HasFlag(TextCase.Hyphenated))
          return textCase | TextCase.KebabCase;

        if (textCase.HasFlag(TextCase.Underscore))
          return textCase | TextCase.SnakeCase;

        if (textCase.HasFlag(TextCase.Dotted))
          return textCase | TextCase.PascalCase;

        if (textCase.HasFlag(TextCase.Spaced))
          return textCase | TextCase.TitleCase;

        if (textCase.HasFlag(TextCase.Joined))
          return textCase | TextCase.PascalCase;
      }

      var hasSeparator = new[] {
        TextCase.Hyphenated,
        TextCase.Underscore,
        TextCase.Dotted,
        TextCase.Spaced,
        TextCase.Joined
      }.Any(@case => textCase.HasFlag(@case));

      if (!hasSeparator)
      {
        if (textCase.HasFlag(TextCase.UpperCase))
          return textCase | TextCase.AllCaps;

        if (textCase.HasFlag(TextCase.LowerCase))
          return textCase | TextCase.SnakeCase;

        if (textCase.HasFlag(TextCase.ProperCase))
          return textCase | TextCase.TitleCase;
      }

      return textCase;
    }

    private static TextCase SelectCase(TextCase textCase)
    {
      if (textCase.HasFlag(TextCase.UpperCase)) return TextCase.UpperCase;
      if (textCase.HasFlag(TextCase.LowerCase)) return TextCase.LowerCase;
      if (textCase.HasFlag(TextCase.ProperCase)) return TextCase.ProperCase;
      return TextCase.KeepOriginal;
    }

    private static string SelectSeparator(TextCase textCase)
    {
      if (textCase.HasFlag(TextCase.Hyphenated)) return "-";
      if (textCase.HasFlag(TextCase.Underscore)) return "_";
      if (textCase.HasFlag(TextCase.Dotted)) return ".";
      if (textCase.HasFlag(TextCase.Spaced)) return " ";
      return "";
    }

    private static void SelectCharacters(TextCase textCase,
      ref char[] wordCharacters, ref char[] preservedCharacters)
    {
      IEnumerable<char> characters = wordCharacters ?? DefaultWordCharacters;
      IEnumerable<char> preserved = preservedCharacters ?? Enumerable.Empty<char>();

      if (preservedCharacters == null)
      {
        preserved = preserved.Union(
          textCase.HasFlag(TextCase.PreserveSymbols)
            ? AllPreservedCharacters
            : DefaultPreservedCharacters
        );
      }

      if (textCase.HasFlag(TextCase.PreserveUnderscore))
      {
        characters = characters.Union('_'.AsSingle());
        preserved = preserved.Union('_'.AsSingle());
      }

      if (textCase.HasFlag(TextCase.PreserveDots))
      {
        preserved = preserved.Union('.'.AsSingle());
      }

      if (textCase.HasFlag(TextCase.PreserveHyphens))
      {
        preserved = preserved.Union('-'.AsSingle());
      }

      characters =
        from c in characters
        let priority = c == '-' ? 1 : 0
        orderby priority
        select c;

      preserved =
        from c in preserved
        let priority = c == '-' ? 1 : 0
        orderby priority
        select c;

      wordCharacters = characters.ToArray();
      preservedCharacters = preserved.ToArray();
    }

    private static IEnumerable<Word> EnumerateWords(string sentence,
      TextCase textCase, char[] wordCharacters, char[] preservedCharacters)
    {
      sentence = SpaceWords(sentence, textCase, preservedCharacters);

      var wordVariants = new string(wordCharacters)
        .Replace("[", @"\[")
        .Replace("]", @"\]");

      var symbolsVariants = new string(preservedCharacters)
        .Replace("[", @"\[")
        .Replace("]", @"\]");

      var word = $@"[\p{{L}}\d{wordVariants}]+";
      var symbols = $@"[{symbolsVariants}]+";
      var other = $@"[^\p{{L}}\d{wordVariants}{symbolsVariants}]";

      if (preservedCharacters.Any())
      {
        var regex = new Regex(
          $@"({symbols})?({word})({symbols})?|(?:^|{other})({symbols})(?:$|{other})"
        );
        var matches = regex.Matches(sentence);
        return
          from match in matches.Cast<Match>()
          let prefix = match.Groups[1].Value
          let text = DiscardCharacters(match.Groups[2].Value,
            wordCharacters.Except(preservedCharacters))
          let suffix = match.Groups[3].Value
          let symbol = match.Groups[4].Value
          select new Word
          {
            IsSymbol = string.IsNullOrEmpty(text),
            Text = NullIfEmpty(text, symbol),
            Prefix = NullIfEmpty(prefix),
            Suffix = NullIfEmpty(suffix)
          };
      }
      else
      {
        var regex = new Regex($"({word})");
        var matches = regex.Matches(sentence);
        return
          from match in matches.Cast<Match>()
          let text = DiscardCharacters(match.Groups[1].Value, wordCharacters)
          select new Word { Text = NullIfEmpty(text) };
      }
    }

    private static string SpaceWords(string sentence, TextCase textCase,
      char[] preservedCharacters)
    {
      Regex regex;

      if (!preservedCharacters.Contains('_'))
      {
        sentence = sentence.Replace('_', ' ');
      }

      if (textCase.HasFlag(TextCase.PreserveAcronyms))
      {
        regex = new Regex(@"([\p{Lu}])(\p{Ll})");
        sentence = regex.Replace(sentence, " $1$2");

        regex = new Regex(@"([\p{Ll}\d])(\p{Lu})");
        while (regex.IsMatch(sentence))
        {
          sentence = regex.Replace(sentence, "$1 $2");
        }
      }
      else
      {
        regex = new Regex(@"([\p{L}\d])(\p{Lu})");
        while (regex.IsMatch(sentence))
        {
          sentence = regex.Replace(sentence, "$1 $2");
        }
      }

      if (!textCase.HasFlag(TextCase.PreserveNumbers))
      {
        regex = new Regex(@"(\p{L})(\d)");
        while (regex.IsMatch(sentence))
        {
          sentence = regex.Replace(sentence, "$1 $2");
        }
      }

      return sentence;
    }

    private static string DiscardCharacters(string sentence, IEnumerable<char> discards)
    {
      discards.ForEach(x => sentence = sentence.Replace(x.ToString(), ""));
      return sentence;
    }

    private static string NullIfEmpty(params string[] tokens)
    {
      return tokens.NotNullOrEmpty().FirstOrDefault();
    }
  }
}