using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.TripRequests
{
    public class TripRequestViewModel : IMapFrom<TripRequest>
    {
        public Trip Trip { get; set; }

        public string TripId { get; set; }

        public string OwnerId { get; set; }

        public ApplicationUser Sender { get; set; }

        public string SenderId { get; set; }
    }
}
