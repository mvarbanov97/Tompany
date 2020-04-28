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
            var random2 = new Random();
            var dateRandom = new Random();
            var trips = new List<Trip>();

            for (int i = 1; i <= 36; i++)
            {
                var car = dbContext.Cars.FirstOrDefault(x => x.Id == i);
                var user = dbContext.Users.FirstOrDefault(x => x.UserName == $"FooUser{i}");
                var randomFromId = random.Next(1, 257);
                var randomToId = random2.Next(1, 257);
                var destinationFrom = dbContext.Destinations.Where(x => x.Id == randomFromId).FirstOrDefault();
                var destinationTo = dbContext.Destinations.Where(x => x.Id == randomToId).FirstOrDefault();
                var dateOfDeparture = new DateTime(2019, dateRandom.Next(1, 12), dateRandom.Next(1, 28));

                var trip = new Trip
                {
                    FromDestination = destinationFrom,
                    ToDestination = destinationTo,
                    FromDestinationName = destinationFrom.Name,
                    ToDestinationName = destinationTo.Name,
                    DateOfDeparture = dateOfDeparture,
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
