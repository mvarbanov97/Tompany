using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Cars;
using Tompany.Web.ViewModels.Destinations;

namespace Tompany.Web.ViewModels.Trips
{
    public class TripEditViewModel : IMapFrom<Trip>
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Моля въведете града, от който тръгвате")]
        [Display(Name = "Град От:")]
        public string FromDestinationName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Моля въведете града, до който пътувате")]
        [Display(Name = "До град:")]
        public string ToDestinationName { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Моля изберете датата, на която ще пътувате")]
        public DateTime DateOfDeparture { get; set; }

        [Display(Name = "Дата на пътуването")]
        public string DateAsString => this.DateOfDeparture.ToString("d");

        [Display(Name = "Цена на пътник")]
        public decimal PricePerPassenger { get; set; }

        [Display(Name = "Допълнителна информация")]
        public string AdditionalInformation { get; set; }

        [Required(ErrorMessage = "Моля посочете автомобила, с който ще пътувате")]
        [Display(Name = "Изберете автомобил")]
        public int CarId { get; set; }

        public IEnumerable<CarDropDownViewModel> Cars { get; set; }

        public IEnumerable<DestinationViewModel> Destinations { get; set; }
    }
}
