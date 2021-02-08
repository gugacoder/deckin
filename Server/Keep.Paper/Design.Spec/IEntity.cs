using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public interface IEntity : IDesign
  {
    IRef Self { get; set; }

    IEnumerable<IEntity> Children();
  }
}
