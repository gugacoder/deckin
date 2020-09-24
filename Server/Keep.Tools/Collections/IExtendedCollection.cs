using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Collections
{
  /// <summary>
  /// Interface de uma coleção com possibilidade de herança e sobreposição e métodos.
  /// </summary>
  /// <typeparam name="object">O tipo da coleção.</typeparam>
  /// <seealso cref="System.Collections.IEnumerable" />
  /// <seealso cref="System.Collections.IList" />
  /// <seealso cref="System.Collections.ICollection" />
  public interface IExtendedCollection : IEnumerable, IList, ICollection
  {
    void AddAt(int index, object item);

    void AddMany(IEnumerable items);

    void AddMany(params object[] items);

    void RemoveRange(int index, int count);

    void RemoveMany(IEnumerable items);

    void RemoveWhen(Predicate<object> match);

    void ForEach(Action<object> action);
  }
}