using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Keep.Tools.Collections
{
  public class PriorityQueue<T> : PriorityQueue<T, T>
    where T : IComparable<T>
  {
    public PriorityQueue()
      : base(value => value)
    {
    }

    public PriorityQueue(IComparer<T> sorter)
      : base(value => value, sorter)
    {
    }

    public PriorityQueue(Comparison<T> sorter)
      : base(value => value, sorter)
    {
    }
  }
}