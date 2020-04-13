using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Trips
{
    public class TripViewModel : IMapFrom<Trip>
    {
        public IEnumerable<TripDetailsViewModel> Trips { get; set; }
    }
}
