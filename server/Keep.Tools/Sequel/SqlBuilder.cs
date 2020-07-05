using Keep.Tools.Collections;
using Keep.Tools.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Keep.Tools.Sequel
{
  public class SqlBuilder : ISettableContract
  {
    public SqlBuilder(string template)
    {
      this.Template = template;
    }

    internal string Template { get; set; }

    internal HashMap Parameters { get; set; }

    public Sql Format(IDbConnection cn,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var dialect = Dialects.GetDialect(cn);
      return Format(dialect, comment, callerName, callerFile, callerLine);
    }

    public Sql Format(Dialect dialect,
      string comment = null,
      [CallerMemberName] string callerName = null,
      [CallerFilePath] string callerFile = null,
      [CallerLineNumber] int callerLine = 0)
    {
      var context = Parameters ?? new HashMap();

      var initialArgs = context.Where(x => Value.IsValidSequelValue(x.Value));

      var target = new Sql
      {
        Text = Template,
        Args = new HashMap(initialArgs)
      };

      BuildStandardTemplate(target, context, dialect);
      BuildExtendedTemplate(target, context, dialect);
      BuildPaginationTemplate(target, context, dialect);
      BuildDebugInformation(target, comment, callerName, callerFile, callerLine);

      return target;
    }

    #region Implementação de ISettable

    HashMap ISettableContract.GetParameters()
      => Parameters;

    HashMap ISettableContract.EnsureParameters()
      => Parameters ??= new HashMap();

    object ISettableContract.GetParameter(string key)
      => Parameters?[key];

    void ISettableContract.SetParameter(string key, object value)
      => (Parameters ??= new HashMap()).Add(key, value);

    #endregion

    #region Algoritmos de substituição de parâmetros

    private static void BuildDebugInformation(Sql target,
      string comment, string callerName, string callerFile, int callerLine)
    {
      if (callerName == null)
        return;

      var path = Path.GetFileNameWithoutExtension(callerFile);
      comment = string.IsNullOrEmpty(comment) ? "" : $"{comment}\n";
      target.Text =
        $"/*\n" +
        $"{path}.{callerName}():{callerLine}\n" +
        $"[{callerFile}]\n" +
        $"{comment}*/\n" +
        $"{target.Text}";
    }

    /// <summary>
    /// Aplica o template simplificado de SQL.
    /// O template suporta a sintaxe:
    /// 
    /// -   @{parametro}
    /// 
    /// Para realizar substituição no texto.
    /// 
    /// Por exemplo, um nome de tabela pode ser definido dinamicamente e declarado
    /// no template como:
    /// 
    ///     SELECT * FROM @{nome_tabela} WHERE id = @id_tabela
    /// 
    /// </summary>
    private static void BuildStandardTemplate(Sql target, HashMap context,
      Dialect dialect)
    {
      if (context?.Any() != true)
        return;

      var isStandardTemplate = target.Text.Contains("@{");
      if (!isStandardTemplate)
        return;

      var parameterNames =
        from name in context.Keys
        orderby name.Length descending, name
        select name;

      foreach (var parameterName in parameterNames)
      {
        var key = $"@{{{parameterName}}}";
        if (target.Text.Contains(key))
        {
          var parameterValue = context[parameterName];
          var value = Change.To<string>(parameterValue);
          target.Text = target.Text.Replace(key, value);
        }
      }
    }

    /// <summary>
    /// Formata o valor da melhor forma possível para compor a SQL ou a
    /// coleção de parâmetros que serão repassados para o executor de SQL.
    /// </summary>
    internal static object CreateSqlCompatibleValue(object value)
    {
      value = (value as Var)?.RawType ?? value;

      if (Value.IsNull(value))
      {
        return DBNull.Value;
      }

      if (value is XNode)
      {
        var xml = ((XNode)value).ToString(SaveOptions.DisableFormatting);
        return xml;
      }

      if (value is SqlBuilder)
      {
        return DBNull.Value;
      }

      if (value is string || value.GetType().IsValueType)
      {
        return value;
      }

      if (value is IEnumerable enumerable)
      {
        var list = enumerable.Cast<object>();

        if (!list.Any())
        {
          return DBNull.Value;
        }

        if (list.First() is byte)
        {
          // Temos um array de bytes.
          // Array de bytes é usado para dados binários.
          // Deve ser repassado como está.
          return list.Cast<byte>().ToArray();
        }

        var text = string.Join(",", list.Select(x => CreateQuotedText(x)));
        return text;
      }

      return DBNull.Value;
    }

    /// <summary>
    /// Obtém uma representação do objeto no formato esperado pelo SQL.
    /// Números são retornados como estão e textos, datas e outros tipos especiais
    /// são retornados entre apóstrofos.
    /// </summary>
    private static string CreateQuotedText(object value)
    {
      if (Value.IsNull(value))
        return "null";

      if (value is string
       || value is DateTime
       || value is TimeSpan
       || value is Uri
       || value is Guid)
        return $"'{value}'";

      if (value is bool bit)
        return bit ? "1" : "0";

      return value.ToString();
    }

    #endregion

    #region Algoritmos de reescrita do template

    /// <summary>
    /// Aplicação do template estendido.
    /// 
    /// 
    /// Visão Geral
    /// -----------
    /// 
    /// Template estendido é uma reescrita da cláusula WHERE de uma SQL
    /// com base no tipo do dado passado para o parâmetro.
    /// 
    /// O tipo do dado passado para o parâmetro é classificado como:
    /// 
    /// -   SmartString
    ///     -   Quando o valor corresponde a um texto.
    /// -   SmartArray
    ///     -   Quando o valor corresponde a uma coleção de qualquer tipo.
    /// -   SmartRange
    ///     -   Quando o valor corresponde a um objeto com as propriedades:
    ///         -   From e To
    ///         -   Start e End
    ///         -   Min e Max
    ///     -   Qualquer objeto passado para o parâmetro que contenha pelo menos
    ///         uma das propriedades acima é considerado SmartRange
    ///     -   Quando o objeto contém apenas a propriedade menor, isto é,
    ///         From, Start ou Min, considera-se um limite "maior ou igual a".
    ///     -   Quando o objeto contém apenas a propriedade maior, isto é,
    ///         To, End ou Max, considera-se um limite "menor ou igual a".
    ///     -   O nome da propriedade é insensível a caso.
    ///     -   Objeto anônimo também pode ser usado.
    /// -   Original
    ///     -   Quando o valor não se encaixa nos tipos acima.
    /// 
    /// O template estendido oferece dois operadores especiais que, quando usados,
    /// produzem uma reescrita da condição. São eles:
    /// 
    /// -   O operator "IS SET"
    /// -   O operador "MATCHES"
    ///     
    /// O operador "is set" é usado para determinar se um valor foi atribuído 
    /// a um parâmetro da SQL, permitindo a SQL se comportar de forma diferente
    /// quando o parâmetro é passado e quando não.
    /// 
    /// O operador "matches" permite a comparação com os diferentes tipos de dados
    /// pela reescrita da SQL. Isto evita a construção dinâmica da SQL para refletir
    /// a escolha de parâmetros.
    /// 
    /// Por exemplo: 
    /// -   Se o valor do parâmetro for um texto uma comparação de igualdade é aplicada.
    /// -   Se o valor contém os curingas "*" ou "?" uma comparação com "LIKE" é aplicada.
    /// -   Se o valor for um vetor de textos uma comparação com "IN" é aplicada.
    /// 
    /// 
    /// Sintaxe: IS SET
    /// ---------------
    /// 
    /// Forma geral:
    /// 
    /// -   @parametro IS [NOT] SET
    ///     
    /// Checa se um valor foi passado para um parâmetro.
    /// 
    /// Exemplos:
    ///     
    ///     --  Se o parametro "id" for informado apenas este usuário será retornado,
    ///     --  esteja ele ativo ou não, caso contrário todos os usuarios ativos serão
    ///     --  retornados.
    ///     SELECT *
    ///       FROM usuario
    ///      WHERE (@id IS SET AND id = @id)
    ///         OR (@id IS NOT SET AND ativo = 1)
    /// 
    /// 
    /// Sintaxe: MATCHES
    /// ----------------
    /// 
    /// Forma geral:
    /// 
    /// -   alvo [NOT] MATCHES [IF SET] @parametro
    /// 
    /// O operador "MATCHES" é um operador especial implementado pelo template para a
    /// reescrita da condição de acordo com o tipo do parâmetro.
    /// 
    /// A correspondência entre tipo e reescrita segue a seguinte forma:
    /// 
    /// -   Para SmartString
    /// 
    ///     -   Quando contém os curingas "*" e "?":
    ///         -   [NOT] alvo LIKE @parametro
    ///         -   O curinga "*" é substituído para "%".
    ///         -   O curinga "?" é substituído para "_".
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = "*FILOP?TOR"
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome LIKE @nome
    ///                 // sendo @nome = '%FILOP_TOR'
    /// 
    ///     -   Para os demais casos:
    ///         -   [NOT] alvo = @parametro
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = "TEA FILOPÁTOR"
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome = @nome
    /// 
    /// -   Para SmartArray
    ///     -   [NOT] alvo IN (valor1, valor2, ...)
    ///     -   O array é transformado em uma lista.
    ///     -   Em caso de array de strings cada string é encapsulada entre apóstrofos.
    ///     -   Exemplo:
    ///     
    ///             SELECT * FROM usuario WHERE nome MATCHES @nome
    ///             // sendo @nome = new [] { "TEA FILOPÁTOR", "FIDÍPEDES" }
    ///             // produz:
    ///             SELECT * FROM usuario WHERE nome IN ('TEA FILOPÁTOR','FIDÍPEDES')
    /// 
    /// -   Para SmartRange
    /// 
    ///     -   Quando o menor e o maior valor são indicados:
    ///         -   [NOT] alvo BETWEEN @min AND @max
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = new { min = "TEA FILOPÁTOR", max = "FIDÍPEDES" }
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome BETWEEN @nome_1 AND @nome_2
    ///             
    ///     -   Quando apenas o menor valor é indicado:
    ///         -   [NOT] alvo &gt;= @min
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = new { min = "TEA FILOPÁTOR" }
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome &gt;= @nome_min
    ///                 
    ///     -   Quando apenas o maior valor é indicado:
    ///         -   [NOT] alvo &lt;= @max
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = new { max = "TEA FILOPÁTOR" }
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE nome &lt;= @nome_max
    ///     
    /// -   Para valores nulo:
    /// 
    ///     -   Quando "IF SET" é indicado:
    ///         -   1=1
    ///         -   Isto faz com que a condição seja simplesmente ignorada.
    ///         -   Ou seja, ou um valor é indicado e a comparação é executada ou um 
    ///             valor não é indicado e a comparação é ignorada.
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES IF SET @nome
    ///                 // sendo @nome = null
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE 1=1
    ///             
    ///     -   Quando "IF SET" não é indicado:
    ///         -   1=0
    ///         -   Isto faz com que a condição sempre falhe.
    ///         -   Ou seja, ou um valor é indicado e a comparação é executada ou um
    ///             valor não é indicado e a comparação é considerada falsa.
    ///         -   Exemplo:
    ///         
    ///                 SELECT * FROM usuario WHERE nome MATCHES @nome
    ///                 // sendo @nome = null
    ///                 // produz:
    ///                 SELECT * FROM usuario WHERE 1=0
    /// 
    /// A cláusula "IF SET" determina o comportamento da comparação quando o valor do
    /// parâmetro não é atribuído:
    /// 
    /// -   Se "IF SET" está ausente a condição é considerada obrigatória e será resolvida como
    ///     falso caso nenhum valor seja atribuído ao parâmetro.
    ///     
    ///     Por exemplo, a instrução:
    ///     -   id MATCHES @id
    ///
    ///     Corresponde a uma condição como:
    ///     -   id = @id
    /// 
    /// -   Se "IF SET" está presente a condição é considerada opcional e será resolvida como
    ///     verdadeiro caso nenhum valor seja atribuído ao parâmetro.
    ///     
    ///     Por exemplo, a instrução:
    ///     -   id MATCHES IF SET @id
    ///
    ///     Corresponde a uma condição parecida com:
    ///     -   (@id IS NULL OR id = @id)
    /// 
    /// 
    /// Sintaxe: MANY MATCHES
    /// ---------------------
    /// 
    /// Forma geral:
    /// 
    /// -   MANY [@parametro] [NOT] MATCHES [IF SET] ( ... )
    /// 
    /// O operador "MANY MATCHES" é um operador especial implementado pelo template para a
    /// reescrita de um bloco com conjuntos diferentes de parâmetros.
    /// 
    /// O parâmetro deve ser uma coleção de coleção de argumentos, isto é:
    /// -   Cada entrada da coleção definida para o parâmetro deve haver uma coleção de argumentos.
    /// 
    /// O algoritmo reescreve o bloco para cada entrada encontrada no mapa, repassando para o
    /// construtor do bloco os parâmetros obtidos daquela entrada.
    /// 
    /// Por exemplo, considere a instrução
    /// 
    ///     WHERE MANY @usuarios MATCHES (nome = @nome AND sobrenome = @sobrenome)
    ///     
    /// E considere a seguinte coleção definida para o parâmetro @usuarios:
    /// 
    ///     object[] mapa =
    ///     {
    ///       new { nome = "fulano", sobrenome = "silva" },
    ///       new { nome = "beltrano", sobrenome = "silva" }
    ///     };
    /// 
    /// A instrução seria reescrita como
    /// 
    ///     WHERE (
    ///       (nome = 'fulano' AND sobrenome = 'silva')
    ///       OR
    ///       (nome = 'beltrano' AND sobrenome = 'silva')
    ///     )
    ///     
    /// A cláusula "IF SET" determina o comportamento da comparação quando o valor do
    /// parâmetro não é atribuído:
    /// 
    /// -   Se "IF SET" está ausente a condição é considerada obrigatória e será resolvida como
    ///     falso caso nenhum valor seja atribuído ao parâmetro.
    ///     
    /// -   Se "IF SET" está presente a condição é considerada opcional e será resolvida como
    ///     verdadeiro caso nenhum valor seja atribuído ao parâmetro.
    ///     
    /// </summary>
    private static void BuildExtendedTemplate(Sql target, HashMap context,
      Dialect dialect)
    {
      ReplaceTemplates(target, context, dialect, new KeyGen());
    }

    /// <summary>
    /// Aplica a substituição dos parâmetros para construção da instrução SQL definitiva.
    /// </summary>
    private static void ReplaceTemplates(Sql target, HashMap context,
      Dialect dialect, KeyGen keyGen)
    {
      ReplaceManyMatches(target, context, dialect, keyGen);

      var extendedParameters =
        ExtractKnownExtendedParameters(target.Text).ToArray();

      var names =
        from name in extendedParameters
        orderby name.Length descending, name
        select name;

      foreach (var name in names)
      {
        var value = context?.Get(name);
        var criteria = CreateCriteria(
          target, context, name, value, dialect, keyGen);

        target.Text = ReplaceMatches(target.Text, name, criteria);
      }

      var patterns = new[] {
        new[] { @"1=1\sand\s1=1", "1=1"},
        new[] { @"1=1\sand\s1=0", "1=0"},
        new[] { @"1=0\sand\s1=1", "1=0"},
        new[] { @"1=0\sand\s1=0", "1=0"},

        new[] { @"1=1\sor\s1=1", "1=1"},
        new[] { @"1=1\sor\s1=0", "1=1"},
        new[] { @"1=0\sor\s1=1", "1=1"},
        new[] { @"1=0\sor\s1=0", "1=0"},

        new[] { @"1=1\sand\s", ""},
        new[] { @"\sand\s1=1", ""},
        new[] { @"1=0\sor\s", ""},
        new[] { @"\sor\s1=0", ""},

        new[] { @"\sand\s\(1=1\)", ""},
        new[] { @"\(1=1\)\sand\s", ""},
        new[] { @"\sor\s\(1=0\)", ""},
        new[] { @"\(1=0\)\sor\s", ""}
      };

      bool changed;
      do
      {
        changed = false;
        foreach (var pattern in patterns)
        {
          var oldText = pattern[0];
          var newText = pattern[1];
          var replacement = Regex.Replace(target.Text, oldText, newText);
          if (replacement != target.Text)
          {
            target.Text = replacement;
            changed = true;
          }
        }
      } while (changed);
    }

    /// <summary>
    /// Aplicação do template estendido para a sintaxe MANY MATCHES:
    /// -   MANY [@parametro] [NOT] MATCHES [IF SET] ( ... )
    /// </summary>
    private static void ReplaceManyMatches(Sql target, HashMap context,
      Dialect dialect, KeyGen keyGen)
    {
      // Aplicando a substituição da instrução:
      //   many [target] [not] matches [if set] ()
      var regex = new Regex(@"[(\s]many\s+(?:@([\w]+)\s+)?(?:(not)\s+)?matches(?:\s+(if\s+set))?\s*?(?:([a-zA-Z_.]+)|([a-zA-Z0-9_]*[(](?>[(](?<c>)|[^()]+|[)](?<-c>))*(?(c)(?!))[)]))");
      var matches = regex.Matches(target.Text);
      foreach (Match match in matches.Cast<Match>().Reverse())
      {
        string replacement = "";

        // Exemplos de textos extraidos
        //
        // " many matches ( ... ) "
        //   [1] ""
        //   [2] ""
        //   [3] ""
        //   -inutil-
        //   [5] "( ... )"
        //
        // " many @parametro not matches if set ( ... ) "
        //   [1] "parametro"
        //   [2] "not"
        //   [3] "if set"
        //   -inutil-
        //   [5] "( ... )"

        var bagName = match.Groups[1].Value;
        var isNot = (match.Groups[2].Length > 0);
        var isOptional = (match.Groups[3].Length > 0);
        var body = match.Groups[5].Value;
        body = body.Substring(1, body.Length - 2);

        var candidate = context.Get(bagName);
        var enumerable = (candidate as IEnumerable)
          ?? ((candidate as Var)?.Value as IEnumerable);
        var bags = enumerable?.OfType<HashMap>();
        if (bags?.Any() != true)
        {
          replacement = isOptional ? "1=1" : "1=0";
        }
        else
        {
          var sentences = new List<string>();
          foreach (var bag in bags)
          {
            var sentence = body;
            var args = new HashMap(context);

            var nestGen = keyGen.Derive();
            foreach (var key in bag.Keys)
            {
              var matches2 = Regex.Matches(sentence, $@"@({key}\w?)");
              foreach (Match match2 in matches2.Cast<Match>().Reverse())
              {
                var matchKey = match2.Groups[1].Value;
                if (matchKey != key)
                  continue;

                var index = match2.Groups[1].Index;
                var count = match2.Groups[1].Length;

                var keyName = nestGen.DeriveName(key);
                var keyValue = bag[key];

                args[keyName] = keyValue;
                sentence = sentence.Inject(index, count, keyName);
              }
            }

            var output = new Sql
            {
              Text = sentence,
              Args = target.Args
            };

            ReplaceTemplates(output, args, dialect, nestGen);
            sentence = output.Text;

            sentence = $"{(isNot ? " not " : "")}({sentence})";
            sentences.Add(sentence);
          }

          replacement = $" ({string.Join("\n or ", sentences.Distinct())})";
        }

        target.Text = target.Text.Inject(
          match.Index, match.Length, $" {replacement}");
      }
    }

    /// <summary>
    /// Constrói a condição que substituirá o template baseado na instrução:
    /// - target [always] matches @parametro
    /// 
    /// Se o valor do parâmetro não tiver sido definido o critério retornado será nulo.
    /// 
    /// O critério criado contém o termo "{0}" que deve ser substituído pela
    /// parte "target" à direta da instrução matches.
    /// 
    /// Por exemplo, em:
    ///   Campo matches @nome
    ///   
    /// O critério produzir pode ser algo como:
    ///   {0} like @nome
    /// 
    /// Que deve então ser substituído com String.Format() pela parte à direta
    /// da instrução, neste caso "Campo":
    ///   String.Format("{0} like @nome", "Campo");
    ///   
    /// Produzindo como resultado:
    ///   Campo like @nome
    /// </summary>
    private static string CreateCriteria(Sql target, HashMap parameters,
      string parameter, object value, Dialect dialect, KeyGen keyGen)
    {
      value = (value as Var)?.RawValue ?? value;

      if (value == null)
      {
        return null;
      }

      if (value is string text)
      {
        var hasWildcard =
          text?.Contains("%") == true || text?.Contains("*") == true ||
          text?.Contains("_") == true || text?.Contains("?") == true;

        if (hasWildcard)
        {
          target.Args[parameter] = text
            .Replace("*", "%")
            .Replace("?", "_");
          return "{0} like @" + parameter;
        }
        else
        {
          target.Args[parameter] = text;
          return "{0} = @" + parameter;
        }
      }

      if (value.GetType().IsValueType)
      {
        target.Args[parameter] = value;
        return "{0} = @" + parameter;
      }

      if (value is SqlBuilder nestSqlBuilder)
      {
        var nestSql = nestSqlBuilder.Format(dialect,
          comment: null, callerName: null, callerFile: null, callerLine: 0);

        var nestGen = keyGen.Derive();
        var nestArgs = nestSql.Args;
        var nestText = nestSql.Text;

        nestText += "\n";

        // cada parametro recebera um sufixo para diferenciacao de parametros
        // que já existem na instância de Sql.
        foreach (var key in nestArgs.Keys)
        {
          var matches = Regex.Matches(nestText, $@"@({key}\w?)");
          foreach (Match match in matches.Cast<Match>().Reverse())
          {
            var matchKey = match.Groups[1].Value;
            if (matchKey != key)
              continue;

            var index = match.Groups[1].Index;
            var count = match.Groups[1].Length;

            var keyName = nestGen.DeriveName(key);
            var keyValue = nestArgs[key];

            target.Args[keyName] = keyValue;
            nestText = nestText.Inject(index, count, keyName);
          }
        }

        return "{0} in (" + nestText + ")";
      }

      if (value is IEnumerable list)
      {
        var keyNames = new List<string>();
        foreach (var item in list)
        {
          var keyName = keyGen.DeriveName(parameter);
          target.Args[keyName] = item;
          keyNames.Add($"@{keyName}");
        }
        return "{0} in (" + string.Join(", ", keyNames) + ")";
      }

      if (value is Range range)
      {
        if (range.Min != null && range.Max != null)
        {
          var minArg = keyGen.DeriveName(parameter);
          var maxArg = keyGen.DeriveName(parameter);
          target.Args[minArg] = range.Min;
          target.Args[maxArg] = range.Max;
          return "{0} between @" + minArg + " and @" + maxArg;
        }

        if (range.Min != null)
        {
          var name = keyGen.DeriveName(parameter);
          target.Args[name] = range.Min;
          return "{0} >= @" + name;
        }

        if (range.Max != null)
        {
          var name = keyGen.DeriveName(parameter);
          target.Args[name] = range.Max;
          return "{0} <= @" + name;
        }

        target.Args[parameter] = value;
        return "{0} = @" + parameter;
      }

      target.Args[parameter] = value;
      return "{0} = @" + parameter;
    }

    /// <summary>
    /// Aplica uma substituição de parâmetro estendido no texto indicado.
    /// 
    /// A função é complemento da função ApplyExtendedTemplate com a responsabilidade
    /// de localizar no texto as ocorrências do padrão "campo = {parametro}" realizando
    /// a substituição pela instrução comparadora definitiva.
    /// 
    /// A porção "{parametro}" pode conter um valor booliano na forma: {parametro|valor}.
    /// Por padrão, quando o valor de um parâmetro não é definido o parâmetro é considerado
    /// bem sucedido pra qualquer registro, tornando nulo o parâmetro para fins de filtro.
    /// Este comportamento pode ser invertido pelo uso de "{parametro|0}" ou "{parametro|false}".
    /// Neste caso, o parâmetro é considerado mal sucedido e todos os registros passam a
    /// ser rejeitados nesta condição.
    /// </summary>
    private static string ReplaceMatches(string sql, string parameter,
      string replacement)
    {
      Regex regex;
      MatchCollection matches;

      // Aplicando a substituição da instrução:
      //   @param is [not] set
      // Que deve ser tornar:
      //   1=1 quando verdadeiro
      //   1=0 quando falso
      regex = new Regex("(@" + parameter + @"\s+is(\s+not)?\s+set)");
      matches = regex.Matches(sql);
      foreach (var match in matches.Cast<Match>())
      {
        var isSet = replacement != null;
        var negate = match.Groups[2].Value.Contains("not");
        if (negate)
          isSet = !isSet;
        var criteria = isSet ? "1=1" : "1=0";
        sql = sql.Replace(match.Value, criteria);
      }

      // Aplicando a substituição da instrução:
      //   target [not] matches [if set] @param
      // Que deve ser substituído por "replacement", produzindo por exemplo:
      //   target = valor
      //   target >= valor
      //   target <= valor
      //   target in (valor, ...)
      //   target like valor
      //   target between valor and valor
      // Sendo que:
      //   quando "not" está presente a condição é invertida.
      //   quando "if set" está presente, se a condicao for nula, o resultado é verdadeiro:
      //     1=1
      //   quando "if set" não está presente, se a condicao for nula, o resultado é falso:
      //     1=0
      regex = new Regex(@"(?:([a-zA-Z_.]+)|([a-zA-Z0-9_]*[(](?>[(](?<c>)|[^()]+|[)](?<-c>))*(?(c)(?!))[)]))\s+(not\s+)?matches\s+(if\s+set\s+)?@" + parameter);
      matches = regex.Matches(sql);
      foreach (var match in matches.Cast<Match>())
      {
        string newSentence = null;

        if (replacement == null)
        {
          // quando "if set" está presente, se a condicao for nula, o resultado é verdadeiro:
          //   1=1
          // quando "if set" não está presente, se a condicao for nula, o resultado é falso:
          //   1=0
          var ifSet = match.Groups[4].Value.Contains("set");
          newSentence = ifSet ? "1=1" : "1=0";
        }
        else
        {
          var leftSide = match.Groups[1].Value + match.Groups[2].Value;
          newSentence = string.Format(replacement, leftSide);

          var negate = match.Groups[3].Value.Contains("not");
          if (negate)
            newSentence = "not " + newSentence;
        }

        sql = sql.Replace(match.Value, newSentence);
      }

      return sql;
    }

    /// <summary>
    /// Extrai todos os nomes de parâmetros na forma "campo = {parametro}" encontrados
    /// no texto.
    /// 
    /// O nome do parâmetro deve ser composto apenas dos caracteres:
    /// -   letras
    /// -   numeros
    /// -   sublinhas
    /// </summary>
    private static IEnumerable<string> ExtractKnownExtendedParameters(string sql)
    {
      var regex = new Regex(@"matches\s+(?:if\s+set\s+)?@([a-zA-Z0-9_]+)|@([a-zA-Z0-9_]+)\s+is(?:\s+not)?\s+set");
      var matches = regex.Matches(sql);
      foreach (var match in matches.Cast<Match>())
      {
        yield return match.Groups[1].Value + match.Groups[2].Value;
      }
    }

    #endregion

    #region Algoritmos de paginação

    /// <summary>
    /// Algoritmo de aplicação do template de paginação.
    /// A paginação tem a forma:
    /// 
    ///   select page (FIELD) [ as ALIAS ]
    ///     ...
    /// paginate [ ALIAS ] by offset OFFSET limit LIMIT
    ///     
    /// Sendo
    /// - "select page (FIELD)" um substituto para "select".
    /// - "page offset" e "page limit" as últimas instruções do comando.
    /// 
    /// Nota:
    /// - Para consultas do SQLServer QUOTED_IDENTIFIER deve estar ativado:
    /// -   SET QUOTED_IDENTIFIER ON;
    /// 
    /// Exemplo:
    /// 
    ///   select page (UserId)
    ///        , UserId
    ///        , Username
    ///        , Password
    ///     from Users
    ///    where Usename like 'A%'
    /// paginate by offset 100 limit 50
    ///     
    /// Exemplo de comando com uso de CTE:
    /// 
    ///   ; with Temp as (select * from Users)
    ///   select page (UserId) as RowNumber
    ///        , UserId
    ///        , Username
    ///        , Password
    ///     from Temp
    ///    where Usename like 'A%'
    /// paginate RowNumber by offset 100 limit 50
    /// 
    /// </summary>
    private static void BuildPaginationTemplate(Sql target, HashMap context,
      Dialect dialect)
    {
      var text = target.Text;
      var flag = RegexOptions.IgnoreCase;

      Regex regex;
      Match match;

      // Numerando linhas com row_number()
      // O trecho:
      //    select page row (FIELD) [ as ALIAS ]
      // Se torna:
      //    select * from ( select row_number() over (order by FIELD) [ as ALIAS ]
      //
      regex = new Regex(
        @"select\s+page\s+row\s+\(\s*[[""]?([\w\d.]+)[]""]?\s*\)(?:\s+as\s+[[""]?([\w\d]+)[]""]?)?"
          , flag);
      while ((match = regex.Match(text)).Success)
      {
        var field = match.Groups[1].Value;
        var alias = match.Groups[2].Value;

        if (string.IsNullOrEmpty(alias))
        {
          alias = "#";
        }

        var replacement =
          $@"select * from (" +
          $@"select row_number() over (order by ""{field}"") as ""{alias}""";

        text = regex.Replace(text, replacement, count: 1);
      }

      // Paginando o resultado
      // O trecho:
      //    paginate by [ ALIAS ] offset OFFSET limit LIMIT
      // Se torna:
      //    ) where [ ALIAS ] between OFFSET and LIMIT
      //
      regex = new Regex(
        @"paginate\s+by(?:\s+[\[""]?([\w\d.]+)[\]""]?)?(?:\s+offset\s+([\w\d@]+)\s+limit\s+([\w\d@]+)|\s+offset\s+([\w\d@]+)|\s+limit\s+([\w\d@]+))"
          , flag);
      while ((match = regex.Match(text)).Success)
      {
        var alias = match.Groups[1].Value;
        var offset = match.Groups[2].Value + match.Groups[4].Value;
        var limit = match.Groups[3].Value + match.Groups[5].Value;

        if (string.IsNullOrEmpty(alias))
        {
          alias = "#";
        }

        string replacement;

        if (!string.IsNullOrEmpty(offset) && !string.IsNullOrEmpty(limit))
        {
          replacement = ") as t where " +
            $@"""{alias}"" between ({offset}+1) and ({offset}+{limit})";
        }
        else if (!string.IsNullOrEmpty(offset))
        {
          replacement = $@") as t where ""{alias}"" > {offset}";
        }
        else if (!string.IsNullOrEmpty(limit))
        {
          replacement = $@") as t where ""{alias}"" <= {limit}";
        }
        else
        {
          replacement = $@") as t";
        }

        text = regex.Replace(text, replacement, count: 1);
      }

      target.Text = text;
    }

    #endregion
  }
}
