using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Trips
{
    public class TripEditViewModel : IMapFrom<Trip>
    {
        public string Id { get; set; }

        public string FromDestinationName { get; set; }

        public string ToDestinationName { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public string AdditionalInformation { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }
    }
}
