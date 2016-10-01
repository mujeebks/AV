using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AVD.Core.Shared.Dtos
{
    public class PersonDto
    {
        public PersonDto()
        {
            IsActive = true;
        }

        /// <summary>
        /// This is the identifier in the external system (e.g. trams profile id for agents and clients, passenger id for companions)
        /// </summary>
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string Title { get; set; }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string MiddleName { get; set; }

		[Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Only used internally at this point - the UI is not expected to see this.
        /// </summary>
        internal bool IsActive { get; set; }
    }
}
