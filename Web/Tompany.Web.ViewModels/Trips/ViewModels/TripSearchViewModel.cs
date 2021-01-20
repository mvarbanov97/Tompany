using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tompany.Web.ViewModels.Trips.ViewModels
{
    public class TripSearchViewModel
    {
        [Display(Name = "From town")]
        public int FromDestinationId { get; set; }

        [Display(Name = "To town")]
        public int ToDestinationId { get; set; }

        [Display(Name = "Date of departure")]
        public DateTime? DateOfDeparture { get; set; }

        public ICollection<TripDetailsViewModel> Trips { get; set; }
    }
}
