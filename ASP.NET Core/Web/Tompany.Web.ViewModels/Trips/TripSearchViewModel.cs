using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Destinations;

namespace Tompany.Web.ViewModels.Trips
{
    public class TripSearchViewModel : IMapFrom<Trip>
    {
        [Display(Name = "От град")]
        public int? FromDestinationId { get; set; }

        [Display(Name = "До град")]
        public int? ToDestinationId { get; set; }

        [Required]
        public string ToDestinationName { get; set; }

        [Required]
        public string FromDestinationName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата на пътуването")]
        public DateTime DateOfDeparture { get; set; }

        public string DateAsString => this.DateOfDeparture.ToString("d");

        public ICollection<TripDetailsViewModel> Trips { get; set; }

        public ICollection<DestinationViewModel> Destinations { get; set; }
    }
}
