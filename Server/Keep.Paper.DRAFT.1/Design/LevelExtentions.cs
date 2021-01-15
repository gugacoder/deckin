using System;
using System.Drawing;
using Microsoft.Extensions.Logging;

namespace Keep.Paper.Design
{
  public static class LevelExtentions
  {
    public static LogLevel ToLogLevel(this Level level)
    {
      switch (level)
      {
        case Level.Trace: return LogLevel.Trace;
        case Level.Default: return LogLevel.Information;
        case Level.Information: return LogLevel.Information;
        case Level.Success: return LogLevel.Information;
        case Level.Warning: return LogLevel.Warning;
        case Level.Danger: return LogLevel.Critical;
        default: return LogLevel.Information;
      }
    }

    public static Color ToColor(this Level level)
    {
      switch (level)
      {
        case Level.Trace: return Color.LightGray;
        case Level.Default: return Color.Black;
        case Level.Information: return Color.DarkBlue;
        case Level.Success: return Color.DarkGreen;
        case Level.Warning: return Color.Orange;
        case Level.Danger: return Color.DarkRed;
        default: return Color.Black;
      }
    }
  }
}
