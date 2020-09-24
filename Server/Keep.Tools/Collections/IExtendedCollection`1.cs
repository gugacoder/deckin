using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Collections
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
  public interface IExtendedCollection<T> : IExtendedCollection, IList<T>, ICollection<T>, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
  {
    void AddAt(int index, T item);

    void AddMany(IEnumerable<T> items);

    void AddMany(params T[] items);

    void RemoveMany(IEnumerable<T> items);

    void RemoveWhen(Predicate<T> match);

    void ForEach(Action<T> action);
  }
}