using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class EmailMap : EntityTypeConfiguration<Email>
    {
        public EmailMap()
        {
            // Primary Key
            this.HasKey(t => t.EmailId);

            // Properties
            this.Property(t => t.ToAddress)
                .IsRequired();

            this.Property(t => t.DocumentLink)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Emails");
            this.Property(t => t.EmailId).HasColumnName("EmailId");
            this.Property(t => t.EmailTypeId).HasColumnName("EmailTypeId");
            this.Property(t => t.ToAddress).HasColumnName("ToAddress");
            this.Property(t => t.SendDate).HasColumnName("SendDate");
            this.Property(t => t.DocumentLink).HasColumnName("DocumentLink");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.IsSuccess).HasColumnName("IsSuccess");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.Body).HasColumnName("Body");

            // Relationships
          
            this.HasRequired(t => t.EmailType)
                .WithMany(t => t.Emails)
                .HasForeignKey(d => d.EmailTypeId);

            this.HasRequired(t => t.User)
                .WithMany(t => t.Emails)
                .HasForeignKey(d => d.UserId);

        }
    }
}
