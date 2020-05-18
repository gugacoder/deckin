using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Serialization.Graph
{
  public interface IPropertyValueSetter
  {
    /// <summary>
    /// Método de deserialização de propriedade.
    /// 
    /// O deserializador invoca este método durante a construção do objeto
    /// para cada propriedade deserializada.
    /// 
    /// O documento (document) repassado contém a estrutura hierárquica das propriedades
    /// deserializadas.
    /// 
    /// O construtor de objetos (builder) pode ser usado para construir automaticamente
    /// uma objeto de um tipo qualquer a partir do documento ou parte dele.
    /// 
    /// Caso o valor da propriedade tenha sido definido o método deve retornar verdadeiro.
    /// O deserializador entende a propriedade como definida e avança na deserialização.
    /// 
    /// Caso falso seja retornado o deserializador tenta outras formas de definir o
    /// valor da propriedade.
    /// </summary>
    /// <param name="property">O nome da propriedade deserializada.</param>
    /// <param name="document">
    /// O documento contendo a estrutura hierárquica das propriedades deserializada.
    /// </param>
    /// <param name="builder">O construtor de objetos.</param>
    /// <returns>
    /// Verdadeiro caso o valor da propriedade tenha sido definido;
    /// Falso caso contrário.
    /// </returns>
    bool SetProperty(string property, NodeModel document, GraphBuilder builder);
  }
}
