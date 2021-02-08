using System;
using Keep.Paper.Design.Spec;
using Keep.Tools.Collections;

namespace Keep.Paper.Design.Rendering
{
  public interface IResponse : IDesign
  {
    bool? Ok { get; set; }

    IEntity Data { get; set; }

    Collection<IEntity> Embedded { get; set; }

    int? Code { get; set; }

    string[] Messages { get; set; }

    string Location { get; set; }
  }
}
