using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Trips.InputModels;

namespace Tompany.Web.ViewModels.Trips.ViewModels
{
    public class TripListViewModel : IMapFrom<Trip>
    {
        public IEnumerable<TripDetailsViewModel> Trips { get; set; }

        public TripSearchInputModel SearchQuery { get; set; }

    }
}
