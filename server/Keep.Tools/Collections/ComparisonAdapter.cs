using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Keep.Tools.Collections
{
  public class ComparisonAdapter<T> : IComparer<T>
  {
    private readonly Comparison<T> comparison;

    public ComparisonAdapter(Comparison<T> comparison)
    {
      this.comparison = comparison;
    }

    public int Compare([AllowNull] T x, [AllowNull] T y)
    {
      return this.comparison.Invoke(x, y);
    }
  }
}
