using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tompany.Web.ViewModels.Cars
{
    public class CarCreateInputModel
    {
        public string CarImageUrl { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Моля въведете марката на автомобила, с който ще пътувате")]
        [Display(Name = "Марка")]
        public string Brand { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Моля въведете модела на автомобила, с който ще пътувате")]
        [Display(Name = "Модел")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Моля посочете годината на производство")]
        [Display(Name = "Година на производство")]
        public int YearOfManufacture { get; set; }

        [Required]
        public string Color { get; set; }

        [Required(ErrorMessage = "Моля посочете сводните места в автомобила")]
        [Range(1, 10)]
        [Display(Name = "Места за пътници")]
        public int Seats { get; set; }

        [Display(Name = "Място за багаж")]
        public bool IsLuggageAvaliable { get; set; }

        [Display(Name = "Пушенето позволено")]
        public bool IsSmokingAllowed { get; set; }

        [Display(Name = "Климатик")]
        public bool IsAirConditiningAvailable { get; set; }

        [Display(Name ="Позволено за любимци")]
        public bool IsAllowedForPets { get; set; }
    }
}
