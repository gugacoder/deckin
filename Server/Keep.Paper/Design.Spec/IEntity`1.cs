using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public interface IEntity<T> : IEntity, IEntityRef, IEntityRef<T>, IDesign
    where T : class, IEntity
  {
  }
}