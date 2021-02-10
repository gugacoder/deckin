using System;
using System.Text;

namespace Keep.Tools
{
  public static class SmallGuid
  {
    private const int GuidLength = 8;

    private static readonly object @lock = new object();
    private static readonly Random random = new Random();
    private static readonly string characters =
      "ABCDEFGHIJKLMNPQRSTUVWXYabcdefghjklmnopqrstuvwxyz0123456789";

    public static string Generate()
    {
      var output = new char[GuidLength];
      for (var i = 0; i < output.Length; i++)
      {
        lock (@lock)
        {
          var index = random.Next(characters.Length);
          output[i] = characters[index];
        }
      }
      return new string(output);
    }
  }
}
