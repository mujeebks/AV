using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_EntityType_HierarchyMap : EntityTypeConfiguration<MD_EntityType_Hierarchy>
    {
        public MD_EntityType_HierarchyMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("MD_EntityType_Hierarchy");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ParentEntityTypeID).HasColumnName("ParentEntityTypeID");
            this.Property(t => t.ChildEntityTypeID).HasColumnName("ChildEntityTypeID");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
        }
    }
}
