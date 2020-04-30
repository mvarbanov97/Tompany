using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.WatchListTrips
{
    public class WatchListTripViewModel : IMapFrom<WatchListTrip>
    {
        public string TripId { get; set; }

        public string TripFromDestinationName { get; set; }

        public string TripToDestinationName { get; set; }

        [DataType(DataType.Date)]
        public DateTime TripDateOfDeparture { get; set; }

        public string UserId { get; set; }

        public string UserUserName { get; set; }
    }
}
