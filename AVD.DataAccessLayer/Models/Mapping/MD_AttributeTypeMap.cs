using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_AttributeTypeMap : EntityTypeConfiguration<MD_AttributeType>
    {
        public MD_AttributeTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Caption)
                .HasMaxLength(500);

            this.Property(t => t.DataType)
                .HasMaxLength(50);

            this.Property(t => t.SqlType)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("MD_AttributeType");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Caption).HasColumnName("Caption");
            this.Property(t => t.IsSelectable).HasColumnName("IsSelectable");
            this.Property(t => t.DataType).HasColumnName("DataType");
            this.Property(t => t.SqlType).HasColumnName("SqlType");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.IsNullable).HasColumnName("IsNullable");
        }
    }
}
