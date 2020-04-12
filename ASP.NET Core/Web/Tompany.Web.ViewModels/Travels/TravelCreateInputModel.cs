using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;

namespace Tompany.Web.ViewModels.Travels
{
    public class TravelCreateInputModel
    {
        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public TimeSpan TimeOfDeparture { get; set; }

        public string AdditionalInformation { get; set; }

    }
}
