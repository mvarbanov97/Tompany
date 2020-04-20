using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class UserTrip : BaseModel<string>, IDeletableEntity
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string TripId { get; set; }

        public Trip Trip { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
