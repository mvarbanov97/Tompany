﻿// ReSharper disable VirtualMemberCallInConstructor
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

            this.UserTrips = new HashSet<UserTrip>();
            this.Cars = new HashSet<Car>();
            this.UserReviews = new HashSet<UserReview>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public string UserImageUrl { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<UserTrip> UserTrips { get; set; }

        public virtual ICollection<Car> Cars { get; set; }

        public virtual ICollection<UserReview> UserReviews { get; set; }
    }
}
