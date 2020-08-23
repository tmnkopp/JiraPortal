using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration; 
namespace JiraPortal
{
    public partial class JiraPortalContext : DbContext
    {
        public JiraPortalContext()
        {
        }

        public JiraPortalContext(DbContextOptions<JiraPortalContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {}
    }
}
