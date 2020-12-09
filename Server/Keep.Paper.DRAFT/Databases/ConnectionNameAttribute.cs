using System;
namespace Keep.Hosting.Databases
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
