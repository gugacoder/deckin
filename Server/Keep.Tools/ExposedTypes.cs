using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Keep.Tools
{
  /// <summary>
  /// Utilitário para descoberta dinâmica de classes expostos com
  /// <see cref="ExposeAttribute"></see>
  /// </summary>
  public static class ExposedTypes
  {
    private static Assembly[] _assemblies;

    private static Assembly[] Assemblies
    {
      get
      {
        if (_assemblies == null)
        {
          var exeAssembly = Assembly.GetEntryAssembly();

          var appAssembly = exeAssembly ?? Assembly.GetExecutingAssembly();
          var appPath = System.IO.Path.GetDirectoryName(appAssembly.Location);

          var files = Directory.GetFiles(appPath, "*.dll", SearchOption.TopDirectoryOnly);
          var assemblies = new List<Assembly>(files.Length);

          foreach (var file in files)
          {
            try
            {
              var assembly = Assembly.LoadFrom(file);
              assemblies.Add(assembly);
            }
            catch { /* Nada a fazer. O componente não é compatível com DotNet. */ }
          }

          if (exeAssembly != null)
          {
            assemblies.Add(exeAssembly);
          }

          _assemblies = assemblies.ToArray();
        }
        return _assemblies;
      }
    }

    /// <summary>
    /// Obtém todos os tipos que correspondem ao contrato indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <param name="contractName">Nome do contrato.</param>
    /// <param name="contractType">Tipo do alvo procurado.</param>
    /// <returns>
    /// Todos os tipos expostos. que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes(string contractName, Type contractType)
    {
      return (
        from assembly in Assemblies
        from type in assembly.GetTypes()
        from attribute in type.GetCustomAttributes().OfType<ExposeAttribute>()
        where (contractName == null) || attribute.ContractName == contractName
        where (contractType == null) || contractType.IsAssignableFrom(type)
        select type
      ).Distinct();
    }

    /// <summary>
    /// Obtém todos os tipos que implementam ou estendem o tipo indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <param name="contractName">Nome do contrato.</param>
    /// <returns>
    /// Todos os tipos expostos. que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes(string contractName)
      => GetTypes(contractName, null);

    /// <summary>
    /// Obtém todos os tipos que implementam ou estendem o tipo indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <param name="contractType">O tipo procurado.</param>
    /// <returns>
    /// Todos os tipos expostos. que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes(Type contractType)
      => GetTypes(null, contractType);

    /// <summary>
    /// Obtém todos os tipos que implementam ou estendem o tipo indicado e
    /// que são expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <typeparam name="T">O tipo procurado.</typeparam>
    /// <returns>
    /// Todos os tipos expostos. que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes<T>()
      => GetTypes(null, typeof(T));

    /// <summary>
    /// Obtém todos os tipos que expostos pelo atributo <see cref="ExposeAttribute"/>
    /// </summary>
    /// <returns>
    /// Todos os tipos expostos. que implementam ou estendem o tipo.
    /// </returns>
    public static IEnumerable<Type> GetTypes()
      => GetTypes(null, null);
  }
}