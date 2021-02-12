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
    public static IResponse Normalize(this IResponse response)
    {
      var normalizer = new ResponseNormalizer();
      normalizer.Normalize(response);
      return response;
    }
  }
}
