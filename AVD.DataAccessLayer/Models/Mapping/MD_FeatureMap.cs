using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_FeatureMap : EntityTypeConfiguration<MD_Feature>
    {
        public MD_FeatureMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Caption)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("MD_Feature");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Caption).HasColumnName("Caption");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ModuleID).HasColumnName("ModuleID");
            this.Property(t => t.IsEnable).HasColumnName("IsEnable");
            this.Property(t => t.IsTopNavigation).HasColumnName("IsTopNavigation");
        }
    }
}
