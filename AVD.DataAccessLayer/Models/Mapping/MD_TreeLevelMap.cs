using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_TreeLevelMap : EntityTypeConfiguration<MD_TreeLevel>
    {
        public MD_TreeLevelMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LevelName)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("MD_TreeLevel");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Level).HasColumnName("Level");
            this.Property(t => t.LevelName).HasColumnName("LevelName");
            this.Property(t => t.AttributeID).HasColumnName("AttributeID");
            this.Property(t => t.IsPercentage).HasColumnName("IsPercentage");
        }
    }
}
