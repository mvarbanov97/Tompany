using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Data.Repositories;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels;
using Tompany.Web.ViewModels.Cars.ViewModels;
using Tompany.Web.ViewModels.Destinations.ViewModels;
using Tompany.Web.ViewModels.Trips.ViewModels;
using Xunit;

namespace Tompany.Services.Data.Tests
{
    public class DestinationsServiceTests
    {
        [Fact]
        public async Task GetAllDestinationShouldReturnCorrectValue()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetAllDestinationsDb").Options;
            var dbContext = new ApplicationDbContext(options);

            await dbContext.Destinations.AddRangeAsync(
                            new Destination(),
                            new Destination(),
                            new Destination());

            await dbContext.SaveChangesAsync();

            var destinationRepository = new EfRepository<Destination>(dbContext);
            var tripRepository = new EfDeletableEntityRepository<Trip>(dbContext);

            this.InitializeMapper();

            var service = new DestinationService(destinationRepository, tripRepository);
            var result = service.GetAllDestinationsAsync();
            Assert.Equal(3, result.Count());
            Assert.IsType<DestinationViewModel[]>(result);
        }

        [Fact]
        public async Task GetSearchResultAsyncShouldReturnCorrectDestinations()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetSearchResultDb").Options;
            var dbContext = new ApplicationDbContext(options);

            await dbContext.Destinations.AddRangeAsync(
                                new Destination
                                {
                                    Id = 1,
                                    Name = "Silistra",
                                },
                                new Destination
                                {
                                    Id = 2,
                                    Name = "Sofia",
                                });

            await dbContext.SaveChangesAsync();

            var fromDestination = await dbContext.Destinations.FirstAsync(x => x.Id == 1);
            var toDestination = await dbContext.Destinations.FirstAsync(x => x.Id == 2);

            await dbContext.Trips.AddAsync(
                                new Trip
                                {
                                    FromDestination = fromDestination,
                                    FromDestinationName = fromDestination.Name,
                                    ToDestination = toDestination,
                                    ToDestinationName = toDestination.Name,
                                    DateOfDeparture = DateTime.Now,
                                });

            await dbContext.SaveChangesAsync();

            var destinationRepository = new EfRepository<Destination>(dbContext);
            var tripRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            this.InitializeMapper();

            var service = new DestinationService(destinationRepository, tripRepository);
            var model = await service.GetSearchResultAsync(fromDestination.Id, toDestination.Id, null);

            Assert.IsType<TripSearchViewModel>(model);

        }

        private void InitializeMapper()
           => AutoMapperConfig.RegisterMappings(
               typeof(ErrorViewModel).GetTypeInfo().Assembly,
               typeof(TripSearchViewModel).GetTypeInfo().Assembly,
               typeof(TripDetailsViewModel).GetTypeInfo().Assembly,
               typeof(DestinationViewModel).GetTypeInfo().Assembly);
    }
}
