using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class City : BaseModel<string> , IDeletableEntity
    {
        public City()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [ForeignKey(nameof(State))]
        public string StateId { get; set; }

        public State State { get; set; }

        [Required]
        [ForeignKey(nameof(Country))]
        public string CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<ZipCode> ZipCodes { get; set; } = new HashSet<ZipCode>();

        public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new HashSet<ApplicationUser>();

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
