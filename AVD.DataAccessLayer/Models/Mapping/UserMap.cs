using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AVD.DataAccessLayer.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.FirstName)
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .HasMaxLength(50);

            this.Property(t => t.Username)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Email)
                .HasMaxLength(250);

           
            // Table & Column Mappings
            this.ToTable("UM_User");
            this.Property(t => t.UserId).HasColumnName("ID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Username).HasColumnName("UserName");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.Isactive).HasColumnName("Isactive");
            this.Property(t => t.password_reset).HasColumnName("password_reset");
            this.Property(t => t.last_login).HasColumnName("last_login");
        }
    }
}
