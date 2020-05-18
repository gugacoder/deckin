using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolset.Net
{
  /// <summary>
  /// Coleção dos principais mime types conforme especificados pelo IANA.
  /// 
  /// Referência:
  ///    http://www.iana.org/assignments/media-types/media-types.xhtml
  /// </summary>
  public enum MimeType
  {
    OctetStream,
    PlainText,
    Csv,
    Excel,

    JsonApplication,
    JsonText,
    JsonSiren,

    XmlApplication,
    XmlText,
    XmlSiren
  }
}