using System;
using Microsoft.EntityFrameworkCore;

namespace Director.Connectors
{
  public class DirectorDbContext : DbContext
  {
    public DirectorDbContext(DbContextOptions<DirectorDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      // O ideal seria obter a string conexao daquelas estocadas pelo Papaer
      // para garantir coesão entre os serviços publicados no mesmo ambiente.
      //    var connectionString = ...Paper..GetConnectionString...
      //    optionsBuilder.UseSqlServer(connectionString);
      base.OnConfiguring(optionsBuilder);
    }
  }
}
