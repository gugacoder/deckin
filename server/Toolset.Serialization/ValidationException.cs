﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolset.Serialization
{
  public class ValidationException : SerializationException
  {
    public ValidationException(string message)
      : base(message)
    {
    }

    public ValidationException(string message, Exception cause)
      : base(message, cause)
    {
    }

    public class NotExpectedException : ValidationException
    {
      public NotExpectedException(Node parentNode, Node unexpectedNode, IEnumerable<NodeType> expectation)
        : base(CreateMessage(parentNode, unexpectedNode, expectation))
      {
      }

      private static string CreateMessage(Node parentNode, Node unexpectedNode, IEnumerable<NodeType> expectation)
      {
        var text = "";
        if (unexpectedNode != null)
        {
          text += "Token não esperado: " + unexpectedNode + ". ";
        }
        if (parentNode != null)
        {
          text += "Próximo de: " + parentNode + ". ";
        }
        if (expectation != null && expectation.Any())
        {
          text += "Era esperado um de tokens: " + string.Join(", ", expectation) + ".";
        }
        return text;
      }
    }

  }
}