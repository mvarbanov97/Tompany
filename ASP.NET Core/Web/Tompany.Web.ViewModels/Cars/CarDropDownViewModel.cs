using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Cars
{
    public class CarDropDownViewModel : IMapFrom<Car>
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string CarBrandName => $"{this.Brand} {this.Model}";
    }
}
