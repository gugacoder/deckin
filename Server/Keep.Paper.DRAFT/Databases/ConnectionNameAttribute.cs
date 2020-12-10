using System;
namespace Keep.Paper.Databases
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
    AllowMultiple = false, Inherited = true)]
  public class ConnectionNameAttribute : Attribute
  {
    public ConnectionNameAttribute(string connectionName)
    {
      this.ConnectionName = connectionName;
    }

    public string ConnectionName { get; }
  }
}
