using System;
using Keep.Tools.Collections;

namespace Keep.Paper.Design
{
  public interface ISerializable
  {
    SerializedValueMap Serialize();

    void Deserialize(SerializedValueMap tokens);
  }
}
