namespace Tompany.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Tompany.Data.Common.Models;

    public class Travel : BaseModel<string>, IDeletableEntity
    {
        public Travel()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Passengers = new HashSet<ApplicationUser>();
        }

        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public TimeSpan TimeOfDeparture { get; set; }

        public string AdditionalInformation { get; set; }

        public ICollection<ApplicationUser> Passengers { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
