using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Paper.Helpers;
using Keep.Tools;
using Keep.Tools.Sequel;
using Keep.Tools.Sequel.Runner;
using Keep.Tools.Xml;

#nullable enable

namespace Director
{
  public class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        //var types = EmbeddedPapers.GetTypes();
        //foreach (var type in types)
        //{

        //}


        var assembly = typeof(Program).Assembly;

        var resource = "papers/CadastroDeBasesDoMercadologic.xml";
        using var stream = assembly.GetManifestResourceStream(resource) ?? Stream.Null;

        using var reader = new StreamReader(stream);


        var xml = reader.ReadToEnd();
        var entity = xml.ToXmlObject<Entity>();
        Debug.WriteLine(entity.ToXElement());

        //var x = new View
        //{
        //  Design = new GridDesign
        //  {
        //    AutoRefresh = 1
        //  }
        //};
        //Debug.WriteLine(x.ToXElement());

        //var xml = @"
        //  <View xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
        //    <Design i:type=""GridDesign"">
        //      <AutoRefresh>1</AutoRefresh>
        //      <Kind>grid</Kind>
        //    </Design>
        //  </View>";

        //var view = xml.ToXmlObject<View>();
        //Debug.WriteLine(view.ToXElement());

      }
      catch (Exception ex)
      {
        ex.Trace();
      }
    }
  }
}
