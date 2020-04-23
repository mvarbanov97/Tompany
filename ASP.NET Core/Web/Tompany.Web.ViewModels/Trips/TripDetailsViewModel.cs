using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Cars;

namespace Tompany.Web.ViewModels.Trips
{
    public class TripDetailsViewModel : IMapFrom<Trip>
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string UserUserName { get; set; }

        public decimal PricePerPassenger { get; set; }

        public int CarId { get; set; }

        public ICollection<View> Views { get; set; }

        public CarViewModel Car { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public string AdditionalInformation { get; set; }

        public IEnumerable<ApplicationUser> Passengers { get; set; }

        public IEnumerable<TripRequest> TripRequests { get; set; }
    }
}
