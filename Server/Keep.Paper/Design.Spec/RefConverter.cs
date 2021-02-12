using System;
using Newtonsoft.Json;

namespace Keep.Paper.Design.Spec
{
  public class RefConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return typeof(Ref).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType,
      object existingValue, JsonSerializer serializer)
    {
      var value = (string)reader.Value;
      Ref @ref = Ref.Parse(value);
      return @ref;
    }

    public override void WriteJson(JsonWriter writer, object value,
      JsonSerializer serializer)
    {
      writer.WriteValue(value.ToString());
    }
  }
}
