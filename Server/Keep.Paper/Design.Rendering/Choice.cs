using System;
namespace Keep.Paper.Design.Rendering
{
  /// <summary>
  /// Directives
  //
  /// TYPE/SUBTYPE
  /// A single, precise MIME type, like text/html.
  /// 
  /// TYPE/*
  /// A MIME type, but without any subtype. image/* will match image/png, image/svg, image/gif and any other image types.
  ///
  /// */*
  /// Any MIME type
  ///
  /// ;q= (q-factor weighting)
  /// Any value used is placed in an order of preference expressed using relative quality value called the weight.
  /// It varies from 0 to 1 up to 3 decimal positions.
  /// The higher the value then the higher its priority.
  /// Default is one.
  /// </summary>
  public class Choice
  {
    public string Value { get; set; }

    public decimal Weight { get; set; }

    public decimal WeightModifier =>
      Value?.Contains("*/*") == true
        ? 0.0M
        : Value?.Contains("*/") == true
          ? 0.3M
          : Value?.Contains("/*") == true
            ? 0.6M
            : 1.0M;
  }
}
