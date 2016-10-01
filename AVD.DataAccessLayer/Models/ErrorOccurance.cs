using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;
using AVD.Common.Exceptions;
using AVD.DataAccessLayer.Enums;

namespace AVD.DataAccessLayer.Models
{
    public partial class ErrorOccurance : BaseModel
    {
        [Key]
        public int ErrorOccuranceId { get; set; }
        public int ErrorMessageId { get; set; }
        public int? UserId { get; set; }
        public int? SupportTicketId { get; set; }
        
        public Guid RequestId { get; set; }

        [StringLength(50)]
        public string AppVersion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [IgnoreMap]
        public DateTime DateCreated { get; set; }

        public virtual User User { get; set; }
        public virtual ErrorMessage ErrorMessage { get; set; }
    }
}
