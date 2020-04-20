using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tompany.Web.ViewModels.Cars
{
    public class CarCreateInputModel
    {
        public string CarImageUrl { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int YearOfManufacture { get; set; }

        public string Color { get; set; }

        public int Seats { get; set; }

        public bool IsLuggageAvaliable { get; set; }

        public bool IsSmokingAllowed { get; set; }

        public bool IsAirConditiningAvailable { get; set; }

        public bool IsAllowedForPets { get; set; }
    }
}
