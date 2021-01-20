using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;
using Tompany.Data.Models.Enums;

namespace Tompany.Data.Models
{
    public class TripRequest : BaseModel<string>, IAuditInfo, IDeletableEntity
    {
        public TripRequest()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public Trip Trip { get; set; }

        public string TripId { get; set; }

        public ApplicationUser Sender { get; set; }

        public string SenderId { get; set; }

        public RequestStatus RequestStatus { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
