using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class CountryCode : BaseModel<string>, IDeletableEntity
    {
        public CountryCode()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        public ICollection<Country> Coutries { get; set; } = new HashSet<Country>();

        public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new HashSet<ApplicationUser>();

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
