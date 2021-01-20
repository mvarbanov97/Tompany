using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Destinations.ViewModels
{
    public class DestinationViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Region { get; set; }

        public string Name { get; set; }

    }
}
