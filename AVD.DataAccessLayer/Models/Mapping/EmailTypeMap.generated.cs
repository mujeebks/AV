using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AVD.DataAccessLayer.Models;

namespace TE.DataAccessLayer.Models.Mapping
{
    public class EmailTypeMap : EntityTypeConfiguration<EmailType>
    {
        public EmailTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.EmailTypeId);

            // Properties
            this.Property(t => t.EmailTypeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("EmailTypes");
            this.Property(t => t.EmailTypeId).HasColumnName("EmailTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
