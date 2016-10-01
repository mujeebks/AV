using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;

namespace AVD.DataAccessLayer.Models
{
    public partial class CachedItem : BaseModel
    {
        [Key]
        public int CachedItemId { get; set; }
        public int? UserId { get; set; }
        public string CacheKey { get; set; }
        public string Value { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? ElapsedMinutes { get; set; }
        public int? ExpiryMinutes { get; set; }
        public virtual User User { get; set; }
    }
}
