﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.Cars;

namespace Tompany.Web.ViewModels.Travels
{
    public class TravelCreateInputModel
    {
        public string FromCity { get; set; }

        public string ToCity { get; set; }

        public DateTime DateOfDeparture { get; set; }

        public TimeSpan TimeOfDeparture { get; set; }

        public string AdditionalInformation { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Car")]
        public int CarId { get; set; }

        [Required]
        public string UserId { get; set; }

        public IEnumerable<CarDropDownViewModel> Cars { get; set; }
    }
}
