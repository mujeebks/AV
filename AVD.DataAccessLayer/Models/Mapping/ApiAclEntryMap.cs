using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class ApiAclEntryMap : EntityTypeConfiguration<ApiAclEntry>
    {
        public ApiAclEntryMap()
        {
            // Primary Key
            this.HasKey(t => t.ApiAclEntryId);

            // Properties
            this.Property(t => t.Path)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("ApiAclEntries");
            this.Property(t => t.ApiAclEntryId).HasColumnName("ApiAclEntryId");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.RoleId).HasColumnName("RoleId");

            // Relationships
            this.HasRequired(t => t.Role)
                .WithMany(t => t.ApiAclEntries)
                .HasForeignKey(d => d.RoleId);

        }
    }
}
