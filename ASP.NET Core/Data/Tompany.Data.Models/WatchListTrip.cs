using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class WatchListTrip : BaseDeletableModel<int>
    {
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string TripId { get; set; }

        public Trip Trip { get; set; }
    }
}
