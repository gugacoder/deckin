using System;
using System.Linq;
using System.Text.RegularExpressions;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Rendering
{
  public class ChoiceCollection : Collection<Choice>
  {
    public string Search(string pattern)
    {
      var regex = new Regex(pattern);
      var choices =
        from x in this
        where regex.IsMatch(x.Value)
        orderby x.WeightModifier descending
        orderby x.Weight descending
        select x;
      var choice = choices.FirstOrDefault();
      return choice?.Value;
    }
  }
}
