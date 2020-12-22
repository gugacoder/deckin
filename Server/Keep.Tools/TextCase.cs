using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools
{
  [Flags]
  public enum TextCase
  {
    Default = 0x000,
    KeepOriginal = 0x001,

    //
    // capitalização
    //
    UpperCase = 0x002,
    LowerCase = 0x004,
    ProperCase = 0x008,
    StartUpperCase = 0x010,
    StartLowerCase = 0x020,

    //
    // separação
    //
    Hyphenated = 0x040,
    Underscore = 0x080,
    Dotted = 0x100,
    Spaced = 0x200,
    Joined = 0x400,

    //
    // composições
    //

    /// <summary>
    /// Todas as palavras em maiúsculo com sublinha separando-as.
    /// </summary>
    AllCaps = UpperCase | Underscore,

    /// <summary>
    /// Todas as palavras em minúsculo com sublinha separando-as.
    /// </summary>
    SnakeCase = LowerCase | Underscore,

    /// <summary>
    /// Todas as palavras em minúsculo com hífen separando-as.
    /// </summary>
    KebabCase = LowerCase | Hyphenated,

    /// <summary>
    /// Palavras iniciando em maiúsculo sem espaço entre elas.
    /// </summary>
    PascalCase = ProperCase | Joined,

    /// <summary>
    /// Palavras iniciando em maiúsculo, com primeira iniciando em minúsculo
    /// e sem espaço entre elas.
    /// </summary>
    CamelCase = ProperCase | Joined | StartLowerCase,

    /// <summary>
    /// Palavras iniciando em maiúsculo e separadas por espaço.
    /// </summary>
    TitleCase = ProperCase | Spaced,

    /// <summary>
    /// Palavras iniciando em minúscula, com primeira iniciando em maiúscula e
    /// separadas por espaço.
    /// </summary>
    SentenceCase = LowerCase | Spaced | StartUpperCase,

    //
    // opções
    //

    /// <summary>
    /// Desativa a preservação de delimitadores antes e depois.
    /// Por exemplo: "__id" se torna "id".    
    /// </summary>
    NoPrefix = 0x1000,

    /// <summary>
    /// Preserva os caracters não-texto.
    /// </summary>
    PreserveSymbols = 0x2000,

    /// <summary>
    /// Mantém números como parte de palavras.
    /// </summary>
    PreserveNumbers = 0x4000,

    /// <summary>
    /// Preserva os caracters não-texto.
    /// </summary>
    PreserveDiacritics = 0x8000,

    /// <summary>
    /// Preserva acrônimos capitalizados, como HTML, IP, etc.
    /// </summary>
    PreserveAcronyms = 0x10000,

    /// <summary>
    /// Preserva sublinhas. Considera sublinha como parte da palavra.
    /// </summary>
    PreserveUnderscore = 0x20000,

    /// <summary>
    /// Preserva sublinhas. Considera sublinha como parte da palavra.
    /// </summary>
    PreserveDots = 0x40000,

    /// <summary>
    /// Preserva sublinhas. Considera sublinha como parte da palavra.
    /// </summary>
    PreserveHyphens = 0x80000,

    /// <summary>
    /// Preserva sublinhas. Considera sublinha como parte da palavra.
    /// </summary>
    PreserveDelimiters = PreserveDots | PreserveHyphens,

    /// <summary>
    /// Preserva símbolos especiais, trata números como parte de palavras e
    /// preserva acrônimos.
    /// </summary>
    Preserve =
        PreserveSymbols
      | PreserveNumbers
      | PreserveAcronyms,
  }
}
