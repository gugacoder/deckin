using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keep.Tools.Collections
{
  /// <summary>
  /// Implementação de uma coleção com possibilidade de herança e sobreposição e métodos.
  /// </summary>
  /// <typeparam name="object">O tipo da coleção.</typeparam>
  /// <seealso cref="System.Collections.IEnumerable" />
  /// <seealso cref="System.Collections.IList" />
  /// <seealso cref="System.Collections.ICollection" />
  public class Collection : IExtendedCollection
  {
    private readonly System.Collections.Generic.List<object> list;
    private readonly ItemStore store;
    private readonly object @lock = new object();

    public Collection()
    {
      this.list = new System.Collections.Generic.List<object>();
      this.store = new ItemStore(this.list);
    }

    public Collection(int capacity)
    {
      this.list = new System.Collections.Generic.List<object>(capacity);
      this.store = new ItemStore(this.list);
    }

    public Collection(IEnumerable items)
    {
      this.list = new System.Collections.Generic.List<object>(items.Cast<object>().Count());
      this.store = new ItemStore(this.list);
      OnCommitAdd(store, items);
    }

    #region Métodos de INTERCEPTAÇÃO de inserção

    /// <summary>
    /// Método de inserção de itens na coleção.
    /// É garantido que qualquer tentativa de adicionar elementos na coleção
    /// será feita por este método.
    /// Uma subclasse pode reescrever este método para modificar o comportamento
    /// de inserção de itens na coleção.
    /// </summary>
    /// <param name="store">A instância de lista na qual os itens devem ser inseridos.</param>
    /// <param name="items">Os itens que estão sendo adicionados à coleção.</param>
    /// <param name="index">
    /// Um índice opcional a partir do qual é esperado que os itens sejam inseridos.
    /// Quando índice for -1 é esperado que os itens sejam adicionados no fim da coleção.
    /// </param>
    protected virtual void OnCommitAdd(ItemStore store, IEnumerable items, int index = -1)
    {
      if (index == -1 || index >= store.Count)
      {
        store.AddMany(items);
      }
      else
      {
        foreach (var item in items)
        {
          store.AddAt(index++, item);
        }
      }
    }

    #endregion

    #region Métodos de INSERÇÃO e REMOÇÂO

    public virtual object this[int index]
    {
      get => list[index];
      set
      {
        if (IsReadOnly)
          throw new UnmodifiableException("A coleção não pode ser modificada.");
        lock (@lock)
        {
          list.RemoveAt(index);
          OnCommitAdd(store, value.AsSingle(), index);
        }
      }
    }

    public virtual void Add(object item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) OnCommitAdd(store, item.AsSingle());
    }

    public virtual void AddAt(int index, object item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) OnCommitAdd(store, item.AsSingle(), index);
    }

    public virtual void Replace(object item, object other)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      if (Equals(item, other))
        return;

      lock (@lock)
      {
        int index;
        while ((index = list.IndexOf(item)) >= 0)
        {
          list.RemoveAt(index);
          OnCommitAdd(store, other.AsSingle(), index);
        }
      }
    }

    public virtual void ReplaceAt(int index, object other)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock)
      {
        list.RemoveAt(index);
        OnCommitAdd(store, other.AsSingle(), index);
      }
    }

    public virtual object PeekFirst()
    {
      return this[0];
    }

    public virtual object PeekLast()
    {
      return this[Count - 1];
    }

    public virtual void AddFirst(object item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) OnCommitAdd(store, item.AsSingle(), 0);
    }

    public virtual void AddLast(object item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) OnCommitAdd(store, item.AsSingle(), Count);
    }

    public virtual void AddMany(IEnumerable items)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) OnCommitAdd(store, items);
    }

    public virtual void AddMany(params object[] items)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) OnCommitAdd(store, items);
    }

    public virtual void Clear()
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) list.Clear();
    }

    public virtual bool Remove(object item)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) return list.Remove(item);
    }

    public virtual object RemoveAt(int index)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock)
      {
        var item = list[index];
        list.RemoveAt(index);
        return item;
      }
    }

    public virtual object RemoveFirst()
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock)
      {
        var item = this[0];
        list.RemoveAt(0);
        return item;
      }
    }

    public virtual object RemoveLast()
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock)
      {
        var item = this[Count - 1];
        list.RemoveAt(Count - 1);
        return item;
      }
    }

    public virtual void RemoveRange(int index, int count)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock)
      {
        var item = list[index];
        list.RemoveRange(index, count);
      }
    }

    public virtual void RemoveMany(IEnumerable items)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock)
      {
        foreach (var item in items)
        {
          list.Remove(item);
        }
      }
    }

    public virtual void RemoveWhen(Predicate<object> match)
    {
      if (IsReadOnly)
        throw new UnmodifiableException("A coleção não pode ser modificada.");

      lock (@lock) list.RemoveAll(match);
    }

    public virtual object[] Find(Func<IEnumerable, IEnumerable> search)
    {
      lock (@lock) return search.Invoke(list).Cast<object>().ToArray();
    }

    public virtual void ForEach(Action<object> action)
    {
      lock (@lock) list.ForEach(action);
    }

    public IEnumerator GetEnumerator() => list.GetEnumerator();

    #endregion

    #region Outros métodos

    public virtual bool IsReadOnly { get; protected set; }

    public virtual int Count => list.Count;

    public virtual bool Contains(object item)
    {
      lock (@lock) return list.Contains(item);
    }

    public virtual void CopyTo(object[] array, int arrayIndex)
    {
      lock (@lock) list.CopyTo(array, arrayIndex);
    }

    public virtual int IndexOf(object item)
    {
      lock (@lock) return list.IndexOf(item);
    }

    #endregion

    #region Redirecionamentos de interface

    object IList.this[int index]
    {
      get => this[index];
      set => this[index] = (object)value;
    }

    int ICollection.Count => Count;

    int IList.Add(object value)
    {
      Add((object)value);
      return IndexOf((object)value);
    }

    void IList.Clear() => Clear();

    bool IList.Contains(object value) => Contains((object)value);

    int IList.IndexOf(object value) => IndexOf((object)value);

    void IList.Insert(int index, object value) => AddAt(index, (object)value);

    void IList.Remove(object value) => Remove((object)value);

    void IList.RemoveAt(int index) => RemoveAt(index);

    #endregion

    #region Implementação específica de interfaces

    void ICollection.CopyTo(Array array, int index) => ((IList)list).CopyTo(array, index);

    bool IList.IsFixedSize => ((IList)list).IsFixedSize;

    object ICollection.SyncRoot => ((IList)list).SyncRoot;

    bool ICollection.IsSynchronized => ((IList)list).IsSynchronized;

    #endregion

    /// <summary>
    /// Repositório de estocagem dos itens da coleção.
    /// Usado como ponto de acesso ao estoque de itens durante sobrescrita de
    /// operações da coleção.
    /// </summary>
    protected class ItemStore : IEnumerable
    {
      private readonly System.Collections.Generic.List<object> list;

      internal ItemStore(System.Collections.Generic.List<object> list)
        => this.list = list;

      public int Count
        => list.Count;

      public object Get(int index)
        => list[index];

      public void Add(object item)
        => list.Add(item);

      public void AddAt(int index, object item)
        => list.Insert(index, item);

      public void AddMany(IEnumerable items)
        => list.AddRange(items.Cast<object>());

      public void Remove(object item)
        => list.Remove(item);

      public void RemoveAt(int index)
        => list.RemoveAt(index);

      public void RemoveRange(int index, int count)
        => list.RemoveRange(index, count);

      public void RemoveMany(IEnumerable items)
        => items.Cast<object>().ToArray().All(list.Remove);

      public void RemoveWhen(Predicate<object> predicate)
        => list.RemoveAll(predicate);

      public IEnumerator GetEnumerator()
        => list.GetEnumerator();
    }
  }
}