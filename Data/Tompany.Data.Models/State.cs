using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class State : BaseModel<string>, IDeletableEntity
    {
        public State()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        // [Required]
        [ForeignKey(nameof(Country))]
        public string CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<City> Cities { get; set; } = new HashSet<City>();

        public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new HashSet<ApplicationUser>();

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
