using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class ErrorMap : EntityTypeConfiguration<Error>
    {
        public ErrorMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("Error");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.StackTrace).HasColumnName("StackTrace");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
        }
    }
}
