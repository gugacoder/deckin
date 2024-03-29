﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Collections
{
  /// <summary>
  /// Extensão de funcionalidades para IEnumerable.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Inicializa uma sequencia de números entre 0 e o número indicado.
    /// </summary>
    public static IEnumerable<int> AsRange(this int number)
    {
      return Enumerable.Range(0, number);
    }

    /// <summary>
    /// Emite um enumerado contendo apenas o objeto referenciado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto.</typeparam>
    /// <param name="single">A instância do objeto.</param>
    /// <returns>Um enumerado contendo o objeto.</returns>
    public static IEnumerable<T> AsSingle<T>(this T single)
    {
      yield return single;
    }

    /// <summary>
    /// Tenta converter o item para o tipo indicado.
    /// Uma exceção é lançada se o tipo não for conversível destino.
    /// </summary>
    /// <typeparam name="T">O tipo destino da conversão.</typeparam>
    /// <param name="enumerable">Os itens a serem convertidos.</param>
    /// <returns>O enumerado dos itens convertidos.</returns>
    public static IEnumerable<TType> ChangeTo<TType>(this IEnumerable enumerable)
    {
      return enumerable.Cast<object>().Select(x => Change.To<TType>(x));
    }

    /// <summary>
    /// Exclui uma instância do enumrado.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto.</typeparam>
    /// <param name="enumerable">O enumerador de objetos.</param>
    /// <param name="single">O objeto a ser adicionado</param>
    /// <returns>O enumerado sem o objeto indicado.</returns>
    public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T single)
    {
      return enumerable.Except(new[] { single });
    }

    /// <summary>
    /// Adiciona um item ao início de um enumerado.
    /// </summary>
    /// <typeparam name="T">Tipo do enumerado.</typeparam>
    /// <param name="instance">Instância adicionado ao início do enumerado.</param>
    /// <returns>O enumerado contendo o item adcionado.</returns>
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T instance)
    {
      return new[] { instance }.AsEnumerable<T>().Concat(enumerable);
    }

    /// <summary>
    /// Resulta um enumerado vazio caso o enumerado origem seja nulo.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>O enumerado origem ou um enumerado vazio.</returns>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
    {
      return enumerable ?? Enumerable.Empty<T>();
    }

    /// <summary>
    /// Emite apenas os itens não nulos.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao-nulos.</returns>
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable)
    {
      return (enumerable == null)
        ? Enumerable.Empty<T>()
        : enumerable.Where(x => x != null && !(x is DBNull));
    }

    /// <summary>
    /// Emite apenas os itens não nulos e não vazios.
    /// Texto nulo ou vazio é considerado vazio.
    /// Vetores e coleções nulas ou vazias são consideradas vazias.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao-vazios.</returns>
    public static IEnumerable<T> NotNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
      if (enumerable == null)
        return Enumerable.Empty<T>();

      return enumerable.Where(x =>
      {
        if (x == null || x is DBNull)
          return false;

        if (x is string)
          return !string.IsNullOrEmpty(x as string);

        if (x is IEnumerable)
          // MoveNext é usado para testar se existem itens no enumerado
          return ((IEnumerable)x).GetEnumerator().MoveNext();

        return true;
      });
    }

    /// <summary>
    /// Emite apenas os itens não nulos, não vazios e não em branco.
    /// Texto nulo, vazio ou contendo apenas espaços, tabulações e quebras de linha
    /// são considerados em branco.
    /// Vetores e coleções nulas ou vazias são consideradas em branco.
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerador.</param>
    /// <returns>Um enumerado contendo apenas os itens nao em branco.</returns>
    public static IEnumerable<T> NotNullOrWhitespace<T>(this IEnumerable<T> enumerable)
    {
      if (enumerable == null)
        return Enumerable.Empty<T>();

      return enumerable.Where(x =>
      {
        if (x == null || x is DBNull)
          return false;

        if (x is string)
          return !string.IsNullOrWhiteSpace(x as string);

        if (x is IEnumerable)
          // MoveNext é usado para testar se existem itens no enumerado
          return ((IEnumerable)x).GetEnumerator().MoveNext();

        return true;
      });
    }

    /// <summary>
    /// Emite os itens de uma coleção como um novo enumerado.
    /// Equivale à forma "enumerable.SelectMany(x => x)"
    /// </summary>
    /// <typeparam name="T">O tipo do enumerato.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <returns>Os itens da coleção enumerados.</returns>
    public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
      return enumerable.SelectMany(x => x);
    }

    /// <summary>
    /// Retorna os itens diferentes ignorando variação de caixa.
    /// </summary>
    public static IEnumerable<string> DistinctIgnoreCase(this IEnumerable<string> enumerable)
    {
      return enumerable.Distinct(StringComparer.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Constrói um vetor com os elementros de um enumerado transformados.
    /// </summary>
    /// <typeparam name="TSource">O tipo do elemento do enumerado.</typeparam>
    /// <typeparam name="TResult">O tipo do elemento produzido pela transformação.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="transformation">O algoritmo de transformação de elemento.</param>
    /// <returns>O vetor com os itens transformados.</returns>
    public static TResult[] ToArray<TSource, TResult>(
      this IEnumerable<TSource> enumerable,
      Func<TSource, TResult> transformation)
    {
      return enumerable.Select(transformation).ToArray();
    }

    /// <summary>
    /// Constrói um vetor com os elementros de um enumerado transformados.
    /// O algoritmo de transformação recebe cada elemento do enumerado e seu
    /// índice correspondente.
    /// </summary>
    /// <typeparam name="TSource">O tipo do elemento do enumerado.</typeparam>
    /// <typeparam name="TResult">O tipo do elemento produzido pela transformação.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="transformation">O algoritmo de transformação de elemento.</param>
    /// <returns>O vetor com os itens transformados.</returns>
    public static TResult[] ToArray<TSource, TResult>(
      this IEnumerable<TSource> enumerable,
      Func<TSource, int, TResult> transformation)
    {
      return enumerable.Select(transformation).ToArray();
    }

    /// <summary>
    /// Constrói uma lista com os elementros de um enumerado transformados.
    /// </summary>
    /// <typeparam name="TSource">O tipo do elemento do enumerado.</typeparam>
    /// <typeparam name="TResult">O tipo do elemento produzido pela transformação.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="transformation">O algoritmo de transformação de elemento.</param>
    /// <returns>A lista com os itens transformados.</returns>
    public static List<TResult> ToList<TSource, TResult>(
      this IEnumerable<TSource> enumerable,
      Func<TSource, TResult> transformation)
    {
      return enumerable.Select(transformation).ToList();
    }

    /// <summary>
    /// Constrói uma lista com os elementros de um enumerado transformados.
    /// O algoritmo de transformação recebe cada elemento do enumerado e seu
    /// índice correspondente.
    /// </summary>
    /// <typeparam name="TSource">O tipo do elemento do enumerado.</typeparam>
    /// <typeparam name="TResult">O tipo do elemento produzido pela transformação.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="transformation">O algoritmo de transformação de elemento.</param>
    /// <returns>A lista com os itens transformados.</returns>
    public static List<TResult> ToList<TSource, TResult>(
      this IEnumerable<TSource> enumerable,
      Func<TSource, int, TResult> transformation)
    {
      return enumerable.Select(transformation).ToList();
    }

    /// <summary>
    /// Constrói uma coleção com os elementros de um enumerado transformados.
    /// </summary>
    /// <typeparam name="TSource">O tipo do elemento do enumerado.</typeparam>
    /// <typeparam name="TResult">O tipo do elemento produzido pela transformação.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="transformation">O algoritmo de transformação de elemento.</param>
    /// <returns>A coleção com os itens transformados.</returns>
    public static Collection<TResult> ToCollection<TSource, TResult>(
      this IEnumerable<TSource> enumerable,
      Func<TSource, TResult> transformation)
    {
      return new Collection<TResult>(enumerable.Select(transformation));
    }

    /// <summary>
    /// Constrói uma coleção com os elementros de um enumerado transformados.
    /// O algoritmo de transformação recebe cada elemento do enumerado e seu
    /// índice correspondente.
    /// </summary>
    /// <typeparam name="TSource">O tipo do elemento do enumerado.</typeparam>
    /// <typeparam name="TResult">O tipo do elemento produzido pela transformação.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="transformation">O algoritmo de transformação de elemento.</param>
    /// <returns>A coleção com os itens transformados.</returns>
    public static Collection<TResult> ToCollection<TSource, TResult>(
      this IEnumerable<TSource> enumerable,
      Func<TSource, int, TResult> transformation)
    {
      return new Collection<TResult>(enumerable.Select(transformation));
    }

    /// <summary>
    /// Constrói uma coleção com os elementros de um enumerado transformados.
    /// </summary>
    /// <typeparam name="T">O tipo do elemento do enumerado.</typeparam>
    /// <param name="enumerable">O enumerado.</param>
    /// <param name="transformation">O algoritmo de transformação de elemento.</param>
    /// <returns>A coleção com os itens transformados.</returns>
    public static Collection<T> ToCollection<T>(this IEnumerable<T> enumerable)
    {
      return new Collection<T>(enumerable);
    }

    /// <summary>
    /// Varre o enumerado e executa a ação para cada item encontrado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">O enumerado dos itens.</param>
    /// <param name="action">A ação a ser executada.</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      foreach (var item in enumerable)
      {
        action.Invoke(item);
      }
    }

    /// <summary>
    /// Varre o enumerado e executa a ação para cada item encontrado, repassando como parâmetro o
    /// item do enumerado mais o seu índice..
    /// </summary>
    /// <typeparam name="T">O tipo do enumerado.</typeparam>
    /// <param name="enumerable">O enumerado dos itens.</param>
    /// <param name="action">A ação a ser executada.</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
    {
      var items = enumerable.Select((element, index) => new { element, index });
      foreach (var item in items)
      {
        action.Invoke(item.element, item.index);
      }
    }

    /// <summary>
    /// Ordena o enumerado pelo próprio dado contido nele.
    /// </summary>
    public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable)
    {
      return enumerable.OrderBy(x => x);
    }

    /// <summary>
    /// Ordena o enumerado pelo próprio dado contido nele.
    /// </summary>
    public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> enumerable)
    {
      return enumerable.ThenBy(x => x);
    }

    /// <summary>
    /// Ordena o enumerado pelo próprio dado contido nele.
    /// </summary>
    public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> enumerable)
    {
      return enumerable.OrderByDescending(x => x);
    }

    /// <summary>
    /// Ordena o enumerado pelo próprio dado contido nele.
    /// </summary>
    public static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> enumerable)
    {
      return enumerable.ThenByDescending(x => x);
    }

    /// <summary>
    /// Adiciona itens ao fim de um enumerado.
    /// </summary>
    public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, params T[] items)
    {
      return enumerable.Concat(items);
    }

#if !NETCOREAPP

    /// <summary>
    /// Salta os últimos itens do enumerado.
    /// </summary>
    /// <param name="count">Quantidade de itens saltados no fim do enumerado</param>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> enumerable, int count)
    {
      var queue = new Queue<T>();
      foreach (var item in enumerable)
      {
        if (queue.Count == count)
        {
          yield return queue.Dequeue();
        }
        queue.Enqueue(item);
      }
    }

#endif
  }
}
