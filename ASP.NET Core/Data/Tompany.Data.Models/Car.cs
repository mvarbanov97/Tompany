namespace Tompany.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Tompany.Data.Common.Models;

    public class Car : BaseModel<int>, IDeletableEntity
    {
        public Car()
        {
        }

        public string ImageUrl { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int YearOfManufacture { get; set; }

        public string Color { get; set; }

        public int Seats { get; set; }

        public bool IsLuggageAvaliable { get; set; }

        public bool IsSmokingAllowed { get; set; }

        public bool IsAirConditiningAvailable { get; set; }

        public bool IsAllowedForPets { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string TripId { get; set; }

        public virtual ICollection<Trip> Trips { get; set; }
    }
}
