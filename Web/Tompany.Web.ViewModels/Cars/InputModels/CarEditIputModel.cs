using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tompany.Web.ViewModels.Cars.InputModels
{
    public class CarEditIputModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Car Picture")]
        public IFormFile CarPicture { get; set; }

        public string CarImageUrl { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the brand of the vehicle you are going to travel with.")]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the model of the vehicle you are going to travel with.")]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Please select the year of manufacture.")]
        [Display(Name = "Year of manufacture")]
        public int YearOfManufacture { get; set; }

        [Required]
        public string Color { get; set; }

        [Required(ErrorMessage = "Please select the available seats in the vehicle.")]
        [Range(1, 10)]
        [Display(Name = "Available seats")]
        public int Seats { get; set; }

        [Display(Name = "Space for luggage")]
        public bool IsLuggageAvaliable { get; set; }

        [Display(Name = "Smoking allowed")]
        public bool IsSmokingAllowed { get; set; }

        [Display(Name = "Air conditioning")]
        public bool IsAirConditiningAvailable { get; set; }

        [Display(Name = "Allowed for pets")]
        public bool IsAllowedForPets { get; set; }
    }
}
