using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Keep.Paper.Api;
using Keep.Paper.Api.Types;
using Keep.Paper.Papers;
using Keep.Tools;
using Keep.Tools.Collections;
using Keep.Tools.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace Keep.Paper.Helpers
{
  public class EmbeddedPapers
  {
    private readonly IAudit<EmbeddedPapers> audit;

    public EmbeddedPapers(IAudit<EmbeddedPapers> audit)
    {
      this.audit = audit;
    }

    public ICollection<PaperType> GetTypes()
    {
      var papers = EnumerateTypesAsync().ToListAsync().AsTask().Await();
      return papers;
    }

    public async Task<ICollection<PaperType>> GetTypesAsync()
    {
      return await EnumerateTypesAsync().ToListAsync();
    }

    private async IAsyncEnumerable<PaperType> EnumerateTypesAsync()
    {
      var assemblies = (
        from type in ExposedTypes.GetTypes()
        select type.Assembly
        ).Distinct();

      var manifests =
        from assembly in assemblies
        from name in assembly.GetManifestResourceNames()
        where name.StartsWith("papers/")
           && name.EndsWith(".xml")
        select new { assembly, name };

      foreach (var manifest in manifests)
      {
        var xml = await ReadTemplateAsync(manifest.assembly, manifest.name);
        var paperType = CreatePaperType(xml);
        if (paperType != null)
        {
          yield return paperType;
        }
      }
    }

    private async Task<Template> ReadTemplateAsync(Assembly assembly, string name)
    {
      try
      {
        using var stream = assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(stream);

        var text = await reader.ReadToEndAsync();
        var xml = CanonicalizeXml(text.ToXElement());

        var templateName = xml.Name;
        var templateTypeName = $"{typeof(Template).Namespace}.{templateName}";
        var templateType = Type.GetType(templateTypeName);

        if (templateType == null)
          throw new NotSupportedException($"Template não suportado: {templateName}");

        var template = (Template)xml.ToXmlObject(templateType);
        return template;
      }
      catch (Exception ex)
      {
        audit.LogDanger(
          To.Text(
            "Falhou a tentativa de construir um Paper a partir de um manifesto. ",
            $"Assembly: {assembly.FullName}; Manifesto: {name}",
            ex
          )
        );
        return null;
      }
    }

    private XElement CanonicalizeXml(XElement xml)
    {
      //
      // Modificando tag vazia para representar um booliano.
      // Isso:
      //    <Tag/>
      // Se torna isso:
      //    <Tag>true</Tag>
      //
      xml.Descendants()
        .Where(x => x.IsEmpty)
        .ForEach(tag => tag.Value = "true");

      return xml;
    }

    private PaperType CreatePaperType(Template template)
    {
      return new PaperType
      {
        Catalog = template.Catalog,
        Name = template.Name,
        Type = typeof(TemplatePaper),
        Factory = provider =>
          ActivatorUtilities.CreateInstance<TemplatePaper>(provider, template)
      };
    }
  }
}
