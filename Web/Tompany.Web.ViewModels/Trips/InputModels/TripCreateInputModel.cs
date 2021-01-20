namespace Tompany.Web.ViewModels.Trips.InputModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Tompany.Data.Models;
    using Tompany.Services.Mapping;
    using Tompany.Web.ViewModels.Cars.ViewModels;
    using Tompany.Web.ViewModels.Cars.InputModels;
    using Tompany.Web.ViewModels.Destinations.ViewModels;
    using Tompany.Web.ViewModels.Users.ViewModels;

    public class TripCreateInputModel : IMapFrom<Trip>
    {
        public Destination FromDestination { get; set; }

        public Destination ToDestination { get; set; }

        [Display(Name = "Going from:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the city you are leaving from.")]
        public string FromDestinationName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the city you are going to.")]
        [Display(Name = "Going to:")]
        public string ToDestinationName { get; set; }

        [Display(Name = "Price per passenger")]
        public decimal PricePerPassenger { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please select the date of departure")]
        public DateTime DateOfDeparture { get; set; }

        [Display(Name = "Date of departure")]
        public string DateAsString => this.DateOfDeparture.ToString("d");

        public TimeSpan TimeOfDeparture { get; set; }

        [MaxLength(255)]
        [Display(Name = "Additional Information")]
        public string AdditionalInformation { get; set; }

        public Car Car { get; set; }

        [Range(0, int.MaxValue)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the vehicle you are going to use for this trip.")]
        [Display(Name = "Select vehicle")]
        public int CarId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string UserId { get; set; }

        public IEnumerable<CarDropDownViewModel> Cars { get; set; }

        public IEnumerable<DestinationViewModel> Destinations { get; set; }

        public CarCreateInputModel CarIfNone { get; set; }

        public string GroupName { get; set; }
    }
}
