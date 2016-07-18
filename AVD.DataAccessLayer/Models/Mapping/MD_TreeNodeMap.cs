using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class MD_TreeNodeMap : EntityTypeConfiguration<MD_TreeNode>
    {
        public MD_TreeNodeMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KEY)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Caption)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("MD_TreeNode");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.NodeID).HasColumnName("NodeID");
            this.Property(t => t.ParentNodeID).HasColumnName("ParentNodeID");
            this.Property(t => t.Level).HasColumnName("Level");
            this.Property(t => t.KEY).HasColumnName("KEY");
            this.Property(t => t.AttributeID).HasColumnName("AttributeID");
            this.Property(t => t.Caption).HasColumnName("Caption");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
        }
    }
}
