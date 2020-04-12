// ReSharper disable VirtualMemberCallInConstructor
namespace Tompany.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Tompany.Data.Common.Models;

    using Microsoft.AspNetCore.Identity;
    using Tompany.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.Trips = new HashSet<Trip>();
            this.Cars = new HashSet<Car>();
        }

        public Gender Gender { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<Trip> Trips { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
