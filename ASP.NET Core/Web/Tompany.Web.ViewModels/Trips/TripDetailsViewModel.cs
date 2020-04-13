using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Trips
{
    public class TripDetailsViewModel : IMapFrom<Trip>
    {
        public string UserUserName { get; set; }

        public int CarId { get; set; }

        public string CarBrand { get; set; }

        public string CarModel { get; set; }

        public string Vehicle => $"{this.CarBrand} {this.CarModel}";

        public int CarSeats { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public string AdditionalInformation { get; set; }

    }
}
