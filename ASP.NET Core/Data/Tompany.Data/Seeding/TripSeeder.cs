using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;

namespace Tompany.Data.Seeding
{
    public class TripSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var random = new Random();
            var trips = new List<Trip>();

            for (int i = 1; i <= 36; i++)
            {
                var car = dbContext.Cars.FirstOrDefault(x => x.Id == i);
                var user = dbContext.Users.FirstOrDefault(x => x.UserName == $"FooUser{i}");

                var trip = new Trip
                {
                    FromCity = $"testCity{i}",
                    ToCity = $"toCity{i}",
                    DateOfDeparture = DateTime.Now,
                    PricePerPassenger = i + 10,
                    Car = car,
                    User = user,
                };
                trips.Add(trip);

                var userTrip = new UserTrip
                {
                    TripId = trip.Id,
                    Trip = trip,
                    UserId = user.Id,
                    User = user,
                };

                user.UserTrips.Add(userTrip);
            }

            await dbContext.Trips.AddRangeAsync(trips);
            await dbContext.SaveChangesAsync();
        }
    }
}
