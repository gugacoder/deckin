﻿using System;
using System.Collections.Generic;
using System.Linq;
using Keep.Tools.Collections;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public interface IEntity : IEntityRef, IDesign
  {
    Ref Self { get; set; }

    Collection<Link> Links { get; set; }
  }
}
