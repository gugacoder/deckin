using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolset.Collections;

namespace Toolset.Collections
{
  /// <summary>
  /// Interface de uma coleção com possibilidade de herança e sobreposição e métodos.
  /// </summary>
  /// <typeparam name="T">O tipo da coleção.</typeparam>
  /// <seealso cref="System.Collections.Generic.IList{T}" />
  /// <seealso cref="System.Collections.Generic.ICollection{T}" />
  /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
  /// <seealso cref="System.Collections.IEnumerable" />
  /// <seealso cref="System.Collections.IList" />
  /// <seealso cref="System.Collections.ICollection" />
  /// <seealso cref="System.Collections.Generic.IReadOnlyList{T}" />
  /// <seealso cref="System.Collections.Generic.IReadOnlyCollection{T}" />
  public interface IExtendedCollection<T> : IEnumerable, IList, ICollection, IList<T>, ICollection<T>, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
  {
    void AddAt(int index, T item);

    void AddMany(System.Collections.Generic.IEnumerable<T> items);

    void AddMany(params T[] items);

    void RemoveRange(int index, int count);

    void RemoveMany(System.Collections.Generic.IEnumerable<T> items);

    void RemoveWhen(Predicate<T> match);

    void ForEach(Action<T> action);
  }
}