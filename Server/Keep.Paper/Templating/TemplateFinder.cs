using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Keep.Paper.Api;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Reflection;
using Keep.Tools.Xml;

namespace Keep.Paper.Templating
{
  public class TemplateFinder
  {
    private readonly IAudit audit;

    public TemplateFinder(IAudit audit)
    {
      this.audit = audit;
    }

    public async IAsyncEnumerable<Template> FindEmbeddedTemplatesAsync()
    {
      var assemblies = (
        from type in ExposedTypes.GetTypes()
        select type.Assembly
        ).Distinct();

      var manifests =
        from assembly in assemblies
        from name in assembly.GetManifestResourceNames()
        where name.EndsWith(".paper.xml")
        select new { assembly, name };

      foreach (var manifest in manifests)
      {
        var template = await ReadTemplateAsync(manifest.assembly, manifest.name);
        if (template != null)
        {
          yield return template;
        }
      }
    }

    private async Task<Template> ReadTemplateAsync(Assembly assembly, string name)
    {
      try
      {
        var parser = new TemplateParser();

        using var stream = assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(stream);

        var text = await reader.ReadToEndAsync();
        var xml = text.ToXElement();

        var template = parser.ParseTemplate(xml);

        template.AssemblyName = assembly.FullName.Split(',', ';').First();
        template.ManifestName = name;

        return template;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Falhou a tentativa de construir um Paper a partir de um manifesto. ",
            $"Assembly: {assembly.FullName}; Manifesto: {name}",
            ex
          ),
          GetType()
        );
        return null;
      }
    }
  }
}
