using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AVD.DataAccessLayer.Models;

namespace TE.DataAccessLayer.Models.Mapping
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            // Primary Key
            this.HasKey(t => t.RoleId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ADRoleName)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Roles");
            this.Property(t => t.RoleId).HasColumnName("RoleId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ADRoleName).HasColumnName("ADRoleName");
            this.Property(t => t.RoleTypeId).HasColumnName("RoleTypeId");

            // Relationships
            this.HasOptional(t => t.RoleType)
                .WithMany(t => t.Roles)
                .HasForeignKey(d => d.RoleTypeId);

        }
    }
}
