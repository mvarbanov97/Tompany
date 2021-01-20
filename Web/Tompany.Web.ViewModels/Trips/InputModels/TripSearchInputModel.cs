using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Destinations.ViewModels;
using Tompany.Web.ViewModels.Trips.ViewModels;


namespace Tompany.Web.ViewModels.Trips.InputModels
{
    public class TripSearchInputModel : IMapFrom<Trip>
    {
        [Display(Name = "From town")]
        public int? FromDestinationId { get; set; }

        [Display(Name = "To town")]
        public int? ToDestinationId { get; set; }

        [Required]
        public string ToDestinationName { get; set; }

        [Required]
        public string FromDestinationName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of travel")]
        public DateTime DateOfDeparture { get; set; }

        public string DateAsString => this.DateOfDeparture.ToString("d");

        public ICollection<TripDetailsViewModel> Trips { get; set; }

        public ICollection<DestinationViewModel> Destinations { get; set; }
    }
}
