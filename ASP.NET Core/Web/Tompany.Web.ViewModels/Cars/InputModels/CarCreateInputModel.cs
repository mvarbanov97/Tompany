using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tompany.Data.Models;

namespace Tompany.Web.ViewModels.Cars.InputModels
{
    [BindProperties]
    public class CarCreateInputModel
    {
        public string CarImageUrl { get; set; }

        [Display(Name = "Car Picture")]
        public IFormFile CarPicture { get; set; }

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

        [PersonalData]
        [Display(Name = "Country")]
        [MaxLength(20)]
        [BindProperty]
        public Country Country { get; set; }

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
