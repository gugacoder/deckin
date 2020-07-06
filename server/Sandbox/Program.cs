using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Keep.Tools;
using Keep.Tools.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Director
{
  public class Term //: IComparable<Term>
  {
    public Term(string text) => Text = text;
    public string Text { get; set; }
    public override string ToString() => $"[ {Text} ]";
    public static implicit operator Term(string text) => new Term(text);
    public int CompareTo(Term other) => Text.CompareTo(other.Text);
  }

  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        var queue = new PriorityQueue<Term, string>(x => x.Text);

        queue.Add("Bacaxi");
        queue.Add("Tananana");
        queue.Add("Apple Pie");

        while (queue.TryRemoveFirst(out Term text, "Bacaxi"))
        {
          Debug.WriteLine(text);
        }
      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
