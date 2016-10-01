using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AVD.DataAccessLayer.Models;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class RoleTypeMap : EntityTypeConfiguration<RoleType>
    {
        public RoleTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.RoleTypeId);

            // Properties
            this.Property(t => t.RoleTypeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("RoleTypes");
            this.Property(t => t.RoleTypeId).HasColumnName("RoleTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
