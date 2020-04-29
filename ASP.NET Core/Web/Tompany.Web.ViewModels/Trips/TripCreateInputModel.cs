namespace Tompany.Web.ViewModels.Trips
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Tompany.Data.Models;
    using Tompany.Services.Mapping;
    using Tompany.Web.ViewModels.Cars;
    using Tompany.Web.ViewModels.Destinations;

    public class TripCreateInputModel : IMapFrom<Trip>
    {
        public Destination FromDestination { get; set; }

        public Destination ToDestination { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Моля въведете града, от който тръгвате")]
        [Display(Name = "Град От:")]
        public string FromDestinationName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Моля въведете града, за който пътувате")]
        [Display(Name = "До град:")]
        public string ToDestinationName { get; set; }

        [Display(Name = "Цена на пътник")]
        public decimal PricePerPassenger { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Моля изберете датата, на която ще пътувате")]
        public DateTime DateOfDeparture { get; set; }

        [Display(Name = "Дата на пътуването")]
        public string DateAsString => this.DateOfDeparture.ToString("d");

        public TimeSpan TimeOfDeparture { get; set; }

        [MaxLength(255)]
        [Display(Name = "Допълнителна информация")]
        public string AdditionalInformation { get; set; }

        [Range(0, int.MaxValue)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Моля въведете автомобила, с който ще пътувате")]
        [Display(Name = "Автомобил")]
        public int CarId { get; set; }

        public string UserId { get; set; }

        public IEnumerable<CarDropDownViewModel> Cars { get; set; }

        public IEnumerable<DestinationViewModel> Destinations { get; set; }

        public CarCreateInputModel CarIfNone { get; set; }
    }
}
