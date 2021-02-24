using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class Country : BaseModel<string>, IDeletableEntity
    {
        public Country()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        // [Required]
        [ForeignKey(nameof(CountryCode))]
        public string CountryCodeId { get; set; }

        public CountryCode CountryCode { get; set; }

        public ICollection<State> States { get; set; } = new HashSet<State>();

        public ICollection<City> Cities { get; set; } = new HashSet<City>();

        public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new HashSet<ApplicationUser>();

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
