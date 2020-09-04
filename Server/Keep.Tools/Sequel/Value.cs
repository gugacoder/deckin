using Keep.Tools.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Keep.Tools.Sequel
{
  internal static class Value
  {
    /// <summary>
    /// Diz se um valor pode ser considerado nulo.
    /// O método considera o tipo DBNull como um valor nulo.
    /// </summary>
    /// <param name="value">O valor a ser verificado.</param>
    /// <returns>Verdadeiro se o valor pode ser considerado nulo.</returns>
    public static bool IsNull(object value)
    {
      return (value == null)
          || (value == DBNull.Value)
          || ((value as Var)?.IsNull == true);
    }

    /// <summary>
    /// Determina se o objeto é considerado um grafo.
    /// Um grafo é um objeto qualquer exceto:
    /// -   Valores do tipo Any.
    /// -   Valores considerados simples (IsSimpleValue).
    /// -   Valores considerados enumerado (IsEnumerable).
    /// -   Valores considerados intervalo (IsRange).
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado um grafo; Falso caso contrário.
    /// </returns>
    public static bool IsGraph(object value)
    {
      if (value == null) return false;
      return !(value is Var) && !IsPrimitive(value) && !IsEnumerable(value)
        && !IsRange(value) && !IsMap(value);
    }

    /// <summary>
    /// Determina se o valor é considerado um valor simples.
    /// Um valor simples é um tipo primitivo qualquer ou uma string.
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado simples; Falso caso contrário.
    /// </returns>
    public static bool IsPrimitive(object value)
    {
      if (value == null) return false;
      return (value is string) || (value?.GetType().IsValueType == true);
    }

    /// <summary>
    /// Determina se o valor é considerado um enumerado.
    /// Um enumerado é um tipo qualquer que implementa IEnumerable exceto strings.
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado enumerado; Falso caso contrário.
    /// </returns>
    public static bool IsEnumerable(object value)
    {
      if (value == null) return false;
      return !(value is string) && (value is IEnumerable);
    }

    /// <summary>
    /// Determina se o objeto é considerado um dicionário.
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado um dicionário; Falso caso contrário.
    /// </returns>
    public static bool IsMap(object value)
    {
      return value is IDictionary;
    }

    /// <summary>
    /// Determina se o objeto é considerado um intervalo (Range).
    /// Um intervalo é um objeto qualquer com propriedades Min ou Max.
    /// </summary>
    /// <returns>
    /// Verdadeiro se o tipo for considerado um intervalo (Range); Falso caso contrário.
    /// </returns>
    public static bool IsRange(object value)
    {
      if (value == null) return false;
      return value._Has("min") || value._Has("max");
    }

    /// <summary>
    /// Determina se o valor é válido para ser repassado a um comando SQL.
    /// Nulo é considerado um valor válido.
    /// </summary>
    public static bool IsValidSequelValue(object value)
    {
      value = (value as Var)?.RawValue ?? value;

      if (IsNull(value))
      {
        return true;
      }

      if (IsPrimitive(value))
      {
        return true;
      }

      if (value is XNode)
      {
        return true;
      }

      if (value is IEnumerable enumerable)
      {
        var list = enumerable.Cast<object>();
        if (list.Any() && list.First() is byte)
        {
          // Temos um array de bytes.
          // Array de bytes é usado para dados binários.
          // Deve ser repassado como array de bytes.
          return true;
        }
      }

      // Nenhum outro valor é válido para ser repassado a um comando de SQL.
      return false;
    }

    /// <summary>
    /// Obtém um valor válido para ser repassado a um comando SQL.
    /// Valores inválidos são retornados como DBNull.Value.
    /// </summary>
    public static object GetValidSequelValue(object value)
    {
      value = (value as Var)?.RawValue ?? value;

      if (IsNull(value))
      {
        return DBNull.Value;
      }

      if (IsPrimitive(value))
      {
        return value;
      }

      if (value is XNode)
      {
        var xml = ((XNode)value).ToString(SaveOptions.DisableFormatting);
        return xml;
      }

      if (value is IEnumerable enumerable)
      {
        var list = enumerable.Cast<object>();
        if (list.Any() && list.First() is byte)
        {
          // Temos um array de bytes.
          // Array de bytes é usado para dados binários.
          // Deve ser repassado como array de bytes.
          return list.Cast<byte>().ToArray();
        }
      }

      // Nenhum outro valor é válido para ser repassado a um comando de SQL.
      return DBNull.Value;
    }
  }
}
