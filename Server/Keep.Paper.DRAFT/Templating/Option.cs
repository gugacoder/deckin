using System;
namespace Keep.Paper.Templating
{
  public class Option : Node
  {
    private string _text;
    private string _data;

    public string Text
    {
      get => _text ?? _data;
      set => _text = value;
    }

    public string Data
    {
      get => _data ?? _text;
      set => _data = value;
    }
  }
}
