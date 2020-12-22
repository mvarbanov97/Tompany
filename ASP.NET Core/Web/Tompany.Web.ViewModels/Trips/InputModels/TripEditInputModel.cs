using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Cars.ViewModels;
using Tompany.Web.ViewModels.Destinations.ViewModels;

namespace Tompany.Web.ViewModels.Trips.InputModels
{
    public class TripEditInputModel : IMapFrom<Trip>
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the city you are leaving from.")]
        [Display(Name = "From city:")]
        public string FromDestinationName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the city you are going to.")]
        [Display(Name = "To city:")]
        public string ToDestinationName { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please select the date of departure.")]
        public DateTime DateOfDeparture { get; set; }

        [Display(Name = "Date of departure")]
        public string DateAsString => this.DateOfDeparture.ToString("d");

        [Display(Name = "Price per passenger")]
        public decimal PricePerPassenger { get; set; }

        [Display(Name = "Additional Information")]
        public string AdditionalInformation { get; set; }

        [Required(ErrorMessage = "Please select the vehicle you are using for this trip.")]
        [Display(Name = "Select vehicle")]
        public int CarId { get; set; }

        public IEnumerable<CarDropDownViewModel> Cars { get; set; }

        public IEnumerable<DestinationViewModel> Destinations { get; set; }
    }
}
