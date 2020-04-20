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
            this.Passengers = new HashSet<ApplicationUser>();
            this.Views = new HashSet<View>();
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

        public virtual ICollection<ApplicationUser> Passengers { get; set; }

        public virtual ICollection<View> Views { get; set; }
    }
}
