using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Keep.Paper.Design.Spec;
using Keep.Tools.Reflection;
using Keep.Tools;

namespace Keep.Paper.Design.Spec
{
  public static class ResponseExtensions
  {
    public static void Normalize(this IResponse response)
      => new ResponseNormalizer().Normalize(response);
  }
}
