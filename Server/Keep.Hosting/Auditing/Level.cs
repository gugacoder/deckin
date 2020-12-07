using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Keep.Hosting.Auditing
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum Level
  {
    Trace,        // Cinza
    Default,      // Preto
    Information,  // Azul
    Success,      // Verde
    Warning,      // Laranja
    Danger,       // Vermelho
  }
}
