using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_EntityTypeMap : EntityTypeConfiguration<MD_EntityType>
    {
        public MD_EntityTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Caption)
                .HasMaxLength(500);

            this.Property(t => t.ShortDescription)
                .HasMaxLength(50);

            this.Property(t => t.ColorCode)
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("MD_EntityType");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Caption).HasColumnName("Caption");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ModuleID).HasColumnName("ModuleID");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.ShortDescription).HasColumnName("ShortDescription");
            this.Property(t => t.ColorCode).HasColumnName("ColorCode");
            this.Property(t => t.IsAssociate).HasColumnName("IsAssociate");
            this.Property(t => t.IsRootLevel).HasColumnName("IsRootLevel");
        }
    }
}
