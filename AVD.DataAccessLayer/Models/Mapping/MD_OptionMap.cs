using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_OptionMap : EntityTypeConfiguration<MD_Option>
    {
        public MD_OptionMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Caption)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("MD_Option");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Caption).HasColumnName("Caption");
            this.Property(t => t.AttributeID).HasColumnName("AttributeID");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
        }
    }
}
