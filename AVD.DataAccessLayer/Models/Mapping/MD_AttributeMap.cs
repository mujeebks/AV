using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_AttributeMap : EntityTypeConfiguration<MD_Attribute>
    {
        public MD_AttributeMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Caption)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("MD_Attribute");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Caption).HasColumnName("Caption");
            this.Property(t => t.AttributeTypeID).HasColumnName("AttributeTypeID");
            this.Property(t => t.IsSystemDefined).HasColumnName("IsSystemDefined");
            this.Property(t => t.IsSpecial).HasColumnName("IsSpecial");
            this.Property(t => t.Description).HasColumnName("Description");

            // Relationships
            this.HasRequired(t => t.MD_Attribute2)
                .WithOptional(t => t.MD_Attribute1);
            this.HasRequired(t => t.MD_AttributeType)
                .WithMany(t => t.MD_Attribute)
                .HasForeignKey(d => d.AttributeTypeID);

        }
    }
}
