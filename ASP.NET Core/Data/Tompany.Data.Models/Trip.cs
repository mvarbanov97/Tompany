namespace Tompany.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Tompany.Data.Common.Models;

    public class Trip : BaseModel<string>, IDeletableEntity
    {
        public Trip()
        {
            this.Id = Guid.NewGuid().ToString();
            this.UserTrips = new HashSet<UserTrip>();
            this.TripRequest = new HashSet<TripRequest>();
            this.Views = new HashSet<View>();
            this.Passengers = new HashSet<ApplicationUser>();
        }

        public decimal PricePerPassenger { get; set; }

        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public TimeSpan TimeOfDeparture { get; set; }

        public string AdditionalInformation { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<UserTrip> UserTrips { get; set; }

        public virtual ICollection<TripRequest> TripRequest { get; set; }

        public virtual ICollection<View> Views { get; set; }

        public virtual ICollection<ApplicationUser> Passengers { get; set; }
    }
}
