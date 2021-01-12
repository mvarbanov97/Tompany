using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tompany.Web.ViewModels.Trips.ViewModels
{
    public class TripSearchResultViewModel
    {
        [Display(Name = "From Town")]
        public int FromDestinationId { get; set; }

        [Display(Name = "To Town")]
        public int ToDestinationId { get; set; }

        [Display(Name = "Date of departure")]
        public DateTime? DateOfDeparture { get; set; }

        public IEnumerable<TripDetailsViewModel> Trips { get; set; }
    }
}
