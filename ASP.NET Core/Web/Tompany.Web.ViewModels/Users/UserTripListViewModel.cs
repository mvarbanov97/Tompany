using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Web.ViewModels.Users
{
    public class UserTripListViewModel : IMapFrom<ApplicationUser>
    {
        public IEnumerable<TripDetailsViewModel> Trips { get; set; }
    }
}
