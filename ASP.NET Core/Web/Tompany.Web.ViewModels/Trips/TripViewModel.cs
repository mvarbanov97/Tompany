﻿using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Trips
{
    public class TripViewModel : IMapFrom<Trip>
    {
        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<TripDetailsViewModel> Trips { get; set; }
    }
}
