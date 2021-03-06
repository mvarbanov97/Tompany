﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Common;
using Tompany.Data.Models;

namespace Tompany.Data.Seeding
{
    public class CarSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var random = new Random();

            for (int i = 1; i <= 36; i++)
            {
                var user = dbContext.Users.FirstOrDefault(x => x.UserName == $"FooUser{i}");

                var car = new Car
                {
                    User = user,
                    UserId = user.Id,
                    CarImageUrl = GlobalConstants.NoCarPictureLocation,
                    Brand = $"FooBrand{i}",
                    Model = $"FooModel{i}",
                    Color = $"FooColor{i}",
                    YearOfManufacture = 2000 + random.Next(0, 20),
                    Seats = random.Next(1, 5),
                    IsAirConditiningAvailable = false,
                    IsSmokingAllowed = true,
                    IsAllowedForPets = false,
                    IsLuggageAvaliable = true,
                };

                user.Cars.Add(car);
                await dbContext.Cars.AddAsync(car);
            }
        }
    }
}
