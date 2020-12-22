using System;
using System.Collections.Generic;
using System.Text;

namespace Tompany.Web.ViewModels.Trips.ViewModels
{
    public class TripSearchResultViewModel
    {
        public int FromDestinationId { get; set; }

        public int ToDestinationId { get; set; }

        public DateTime? DateOfDeparture { get; set; }

        public ICollection<TripDetailsViewModel> Trips { get; set; }
    }
}
