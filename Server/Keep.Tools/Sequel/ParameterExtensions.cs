using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Keep.Tools.Collections;
using Keep.Tools.Data;
using Keep.Tools.Reflection;
using static Keep.Tools.Sequel.Value;

namespace Keep.Tools.Sequel
{
  /// <summary>
  /// Extensões de processamento de parâmetros de SQL.
  /// </summary>
  public static class ParameterExtensions
  {
    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no mapa
    /// de chave/valor indicado.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static T Set<T>(this T sql, NameValueCollection parameters)
      where T : ISettable
    {
      if (parameters == null)
        return sql;

      foreach (var key in parameters.AllKeys)
      {
        ((ISettableContract)sql).SetParameter(key, parameters[key]);
      }
      return sql;
    }

    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no mapa
    /// de chave/valor indicado.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static T Set<T>(this T sql, IEnumerable<KeyValuePair<string, object>> parameters)
      where T : ISettable
    {
      if (parameters == null)
        return sql;

      foreach (var parameter in parameters)
      {
        ((ISettableContract)sql).SetParameter(parameter.Key, parameter.Value);
      }
      return sql;
    }

    /// <summary>
    /// Atribui de uma só vez uma série de parâmetros definidos no vetor indicado.
    /// O vetor é interpretado em pares de chave valor.
    /// Iniciando em zero, os termos pares são considerados como chaves e os termos
    /// ímpares são considerados como valores destas chaves.
    /// Exemplo:
    ///     var texto = "select * from usuario where nome = @nome, ativo = @ativo";
    ///     var sql = texto.AsSql();
    ///     sql.Set("nome", "Fulano", "ativo", 1);
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameters">Os parâmetros a ser atribuídos.</param>
    /// <returns>
    /// A mesma instância da SQL obtida como parâmetro para encadeamento
    /// de operações.
    /// </returns>
    public static T Set<T>(this T sql, params object[] parameters)
      where T : ISettable
    {
      if (parameters == null)
        return sql;

      var sample = parameters.NotNull().FirstOrDefault();

      if (IsGraph(sample) || IsMap(sample))
      {
        var invalid = parameters.NotNull().Any(x => !IsGraph(x) && !IsMap(x));
        if (invalid)
          throw new Exception("Todos os parâmetros deveriam ser objetos ou mapas para extração de propriedades, mas foi encontrado: " + invalid.GetType().FullName);

        var target = ((ISettableContract)sql).EnsureParameters();
        foreach (var graph in parameters)
        {
          if (graph is IDictionary map)
          {
            UnwrapMap(map, target);
          }
          else
          {
            UnwrapGraph(graph, target);
          }
        }
      }
      else
      {
        var invalid = parameters.NotNull().FirstOrDefault(x => IsGraph(x));
        if (invalid != null)
        {
          throw new Exception("Era esperado uma lista de parâmetros simples na forma chave/valor mas foi encontrado: " + invalid.GetType().FullName);
        }

        UnwrapParameters(parameters, ((ISettableContract)sql).EnsureParameters());
      }
      return sql;
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, NameValueCollection map,
      params NameValueCollection[] otherMaps)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, map, otherMaps);
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY @bagName MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, string bagName,
      NameValueCollection parameters, params NameValueCollection[] otherMaps)
    {
      if (parameters == null || otherMaps == null)
        return sql;

      var map = ((ISettableContract)sql).EnsureParameters();
      var bag = (map[bagName] as Var)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        map[bagName] = bag = new List<IDictionary<string, object>>();
      }

      var entries =
        parameters.AsSingle().Concat(otherMaps).Select(CreateMap).ToArray();
      bag.AddRange(entries);

      return sql;
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, IDictionary map, params IDictionary[] otherMaps)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, map, otherMaps);
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// A cesta é usada pela instrução "MANY @bagName MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="map">O mapa de parâmetros.</param>
    /// <param name="otherMaps">
    /// Outros mapas de parâmetros.
    /// Cada mapa de parâmetros irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, string bagName,
      IDictionary parameters, params IDictionary[] otherMaps)
    {
      var map = ((ISettableContract)sql).EnsureParameters();
      var bag = (map[bagName] as Var)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        map[bagName] = bag = new List<IDictionary<string, object>>();
      }

      var entries =
        parameters.AsSingle().Concat(otherMaps).Select(CreateMap).ToArray();
      bag.AddRange(entries);

      return sql;
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// Os parâmetros são obtidos das propriedades dos grafos indicados.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="parameter">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherParameters">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, object parameter, params object[] otherParameters)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, "", parameter, otherParameters);
    }

    /// <summary>
    /// Adiciona um conjunto de parâmetros a uma cesta de parâmetros.
    /// Os parâmetros são obtidos das propriedades dos grafos indicados.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="graph">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherGraphs">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, string bagName, object graph, params object[] otherGraphs)
    {
      var graphs = graph.AsSingle().Concat(otherGraphs);

      var invalid = graphs.NotNull().FirstOrDefault(x => !IsGraph(x));
      if (invalid != null)
        throw new Exception("Todos os parâmetros deveriam ser objetos para extração de propriedads mas foi contrado: " + invalid.GetType().FullName);

      var map = ((ISettableContract)sql).EnsureParameters();
      var bag = (map[bagName] as Var)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        map[bagName] = bag = new List<IDictionary<string, object>>();
      }

      foreach (var item in graphs)
      {
        var entries = UnwrapGraph(item);
        bag.Add(entries);
      }

      return sql;
    }

    /// <summary>
    /// Adiciona entradas na cesta de parâmetros e repassa um construtor para definição
    /// dos parâmetros.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="graph">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherGraphs">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, Action<Bag> builder, params Action<Bag>[] otherBuilder)
    {
      // Os itens serão postos na cesta padrão. O nome da cestra padrão é uma string vazia.
      return SetMany(sql, "", builder, otherBuilder);
    }

    /// <summary>
    /// Adiciona entradas na cesta de parâmetros e repassa um construtor para definição
    /// dos parâmetros.
    /// 
    /// A cesta é usada pela instrução "MANY MATCHES" para construir uma cláusula
    /// de condição para cada coleção de parâmetros na cesta.
    /// A cesta é estocada como uma lista de mapas contendo chave/valor,
    /// mais especificamente um IEnumerable de IDictionary.
    /// </summary>
    /// <param name="sql">A SQL a ser processada.</param>
    /// <param name="bagName">Nome da cesta de parâmetros.</param>
    /// <param name="graph">O objeto que será desmontado para obtenção dos parâmetros.</param>
    /// <param name="otherGraphs">
    /// Outros objetos que serão desmontados para obtenção de parâmetros.
    /// Cada objeto desmontado irá compor uma entrada na cesta de parâmetros.
    /// </param>
    /// <returns>A própria instância de Sql processada.</returns>
    public static SqlBuilder SetMany(this SqlBuilder sql, string bagName, Action<Bag> builder, params Action<Bag>[] otherBuilder)
    {
      var map = ((ISettableContract)sql).EnsureParameters();
      var bag = (map[bagName] as Var)?.Value as List<IDictionary<string, object>>;
      if (bag == null)
      {
        map[bagName] = bag = new List<IDictionary<string, object>>();
      }

      var builders = builder.AsSingle().Concat(otherBuilder);
      foreach (var instance in builders)
      {
        var many = new Bag();
        instance.Invoke(many);
        bag.Add(many.Parameters);
      }

      return sql;
    }

    /// <summary>
    /// Converte o dicionario em uma coleção de parâmetros nome/valor.
    /// </summary>
    /// <param name="dictionary">O dicionário a ser convertido.</param>
    /// <returns>O mapa de parâmetros obtido.</returns>
    private static IDictionary<string, object> CreateMap(IDictionary dictionary)
    {
      var map = dictionary as IDictionary<string, object>;
      if (map == null)
      {
        var entries =
          dictionary
            .Cast<DictionaryEntry>()
            .Select(e => new KeyValuePair<string, object>(e.Key.ToString(), e.Value));
        map = new HashMap(entries);
      }
      return map;
    }

    /// <summary>
    /// Converte o dicionario em uma coleção de parâmetros nome/valor.
    /// </summary>
    /// <param name="dictionary">O dicionário a ser convertido.</param>
    /// <returns>O mapa de parâmetros obtido.</returns>
    private static IDictionary<string, object> CreateMap(NameValueCollection dictionary)
    {
      var map = new HashMap();
      foreach (var key in dictionary.AllKeys)
      {
        map[key] = dictionary[key];
      }
      return map;
    }

    /// <summary>
    /// Desmonta um dicionario.
    /// </summary>
    /// <param name="source">O dicionario a ser desmontado.</param>
    /// <param name="target">Mapa para escrita das propriedades encontradas.</param>
    /// <returns>O mapa de propriedades do dicionario.</returns>
    public static IDictionary<string, object> UnwrapMap(IDictionary source)
    {
      var target = new HashMap();
      UnwrapMap(source, target);
      return target;
    }

    /// <summary>
    /// Desmonta um dicionario.
    /// </summary>
    /// <param name="source">O dicionario a ser desmontado.</param>
    /// <param name="target">Mapa para escrita das propriedades encontradas.</param>
    /// <returns>O mapa de propriedades do dicionario.</returns>
    public static void UnwrapMap(IDictionary source,
      IDictionary<string, object> target)
    {
      foreach (string name in source.Keys)
      {
        var value = source[name];
        if (value != null)
        {
          target[name] = IsPrimitive(value)
            ? value : (value as Var ?? new Var(value));
        }
      }
    }

    /// <summary>
    /// Desmonta um objeto em um mapa de propriedades.
    /// </summary>
    /// <param name="source">O grafo a ser desmontado.</param>
    /// <returns>O mapa de propriedades do grafo.</returns>
    public static IDictionary<string, object> UnwrapGraph(object source)
    {
      var target = new HashMap();
      UnwrapGraph(source, target);
      return target;
    }

    /// <summary>
    /// Desmonta um objeto em um mapa de propriedades.
    /// </summary>
    /// <param name="source">O grafo a ser desmontado.</param>
    /// <param name="target">Mapa para escrita das propriedades encontradas.</param>
    /// <returns>O mapa de propriedades do grafo.</returns>
    public static void UnwrapGraph(object source,
      IDictionary<string, object> target)
    {
      foreach (var name in source._Keys())
      {
        var value = source._Get(name);
        if (value != null)
        {
          target[name] = IsPrimitive(value)
            ? value : (value as Var ?? new Var(value));
        }
      }
    }

    /// <summary>
    /// Copia os parâmetros da lista indicada para um mapa.
    /// A lista de parâmetros deve ter um número par de argumentos.
    /// Cada argumento par, iniciando em zero, é considerado um nome de chave.
    /// E cada argumento impar é considerado o valor dessa chave.
    /// </summary>
    /// <param name="target">O dicionário a ser modificado.</param>
    /// <param name="source">A coleção de parâmetros.</param>
    private static void UnwrapParameters(IEnumerable<object> source,
      IDictionary<string, object> target)
    {
      var enumerator = source.GetEnumerator();
      while (enumerator.MoveNext())
      {
        if (enumerator.Current is string)
        {
          var token = (string)enumerator.Current;
          if (token.Contains("="))
          {
            var statements =
              from statement in token.Split(',')
              let tokens = statement.Split('=')
              select new
              {
                key = tokens.First(),
                value = string.Join("=", tokens.Skip(1))
              };

            foreach (var statement in statements)
            {
              var key = statement.key.Trim();
              target[key] = statement.value.Trim();
            }
          }
          else
          {
            var key = token.Trim();

            if (!enumerator.MoveNext())
              throw new Exception("Faltou definir o valor de: " + key);

            target[key] = enumerator.Current;
          }
        }
        else
        {
          throw new Exception(
            $"Era esperado um nome de parâmetro ou uma atribuição " +
            $"(\"parametro=valor\") mas foi encontrado: {enumerator.Current}"
          );
        }
      }
    }
  }
}