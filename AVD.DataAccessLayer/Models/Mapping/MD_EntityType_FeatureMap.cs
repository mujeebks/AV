using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_EntityType_FeatureMap : EntityTypeConfiguration<MD_EntityType_Feature>
    {
        public MD_EntityType_FeatureMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("MD_EntityType_Feature");
            this.Property(t => t.TypeID).HasColumnName("TypeID");
            this.Property(t => t.FeatureID).HasColumnName("FeatureID");
            this.Property(t => t.ID).HasColumnName("ID");

            // Relationships
            this.HasRequired(t => t.MD_EntityType)
                .WithMany(t => t.MD_EntityType_Feature)
                .HasForeignKey(d => d.TypeID);

        }
    }
}
