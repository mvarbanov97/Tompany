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
        private DestinationService destinationsService;
        private ApplicationDbContext dbContext;

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            this.dbContext = new ApplicationDbContext(options);
            var unitOfWork = new UnitOfWork(this.dbContext);
            this.destinationsService = new DestinationService(unitOfWork);

            this.InitializeMapper();
        }

        [Fact]
        public async Task GetAllDestinationShouldReturnCorrectValue()
        {
            await this.InitializeAsync();

            await this.dbContext.Destinations.AddRangeAsync(
                            new Destination(),
                            new Destination(),
                            new Destination());

            await this.dbContext.SaveChangesAsync();

            var result = this.destinationsService.GetAllDestinationsAsync();
            Assert.Equal(3, result.Count());
            Assert.IsType<List<DestinationViewModel>>(result);
        }

        [Fact]
        public async Task GetSearchResultAsyncShouldReturnCorrectDestinations()
        {
            await this.InitializeAsync();

            await this.dbContext.Destinations.AddRangeAsync(
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

            await this.dbContext.SaveChangesAsync();

            var fromDestination = await this.dbContext.Destinations.FirstAsync(x => x.Id == 1);
            var toDestination = await this.dbContext.Destinations.FirstAsync(x => x.Id == 2);

            await this.dbContext.Trips.AddAsync(
                                new Trip
                                {
                                    FromDestination = fromDestination,
                                    FromDestinationName = fromDestination.Name,
                                    ToDestination = toDestination,
                                    ToDestinationName = toDestination.Name,
                                    DateOfDeparture = DateTime.Now,
                                });

            await this.dbContext.SaveChangesAsync();

            var model = await this.destinationsService.GetSearchResultAsync(fromDestination.Id, toDestination.Id, null);

            Assert.IsType<TripSearchViewModel>(model);

        }

        private void InitializeMapper()
           => AutoMapperConfig.RegisterMappings(
               typeof(DestinationViewModel).GetTypeInfo().Assembly,
               typeof(TripSearchViewModel).GetTypeInfo().Assembly,
               typeof(TripDetailsViewModel).GetTypeInfo().Assembly);
    }
}
