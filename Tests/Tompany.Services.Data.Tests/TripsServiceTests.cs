namespace Tompany.Services.Data.Tests
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Tompany.Data;
    using Tompany.Data.Models;
    using Tompany.Data.Repositories;
    using Tompany.Services.Mapping;
    using Tompany.Web.ViewModels;
    using Tompany.Web.ViewModels.Destinations.ViewModels;
    using Tompany.Web.ViewModels.Trips.InputModels;
    using Tompany.Web.ViewModels.Trips.ViewModels;
    using Xunit;

    public class TripsServiceTests
    {
        private TripsService tripsService;
        private ApplicationDbContext dbContext;

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            this.dbContext = new ApplicationDbContext(options);
            var unitOfWork = new UnitOfWork(this.dbContext);
            this.tripsService = new TripsService(unitOfWork);

            this.InitializeMapper();
        }

        [Fact]
        public async Task CreateAsyncShouldCreateCorrectly()
        {
            await this.InitializeAsync();

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                UserName = "Momchil",
            });

            await this.dbContext.SaveChangesAsync();

            await this.tripsService.CreateAsync(new TripCreateInputModel
            {
                ApplicationUser = await this.dbContext.Users.FirstAsync(),
                FromDestinationName = "Silistra",
                ToDestinationName = "Sofia",
            });

            var expectedTrip = new Trip { FromDestinationName = "Silistra", ToDestinationName = "Sofia" };

            var actualTrip = await this.dbContext.Trips.FirstOrDefaultAsync(x => x.FromDestinationName == "Silistra" && x.ToDestinationName == "Sofia");

            Assert.NotNull(actualTrip);
            Assert.Equal(expectedTrip.ToDestinationName, actualTrip.ToDestinationName);
            Assert.Equal(expectedTrip.FromDestinationName, actualTrip.FromDestinationName);
        }

        [Fact]
        public async Task EditAsyncShouldEditCorrectly()
        {
            await this.InitializeAsync();

            var tripId = Guid.NewGuid().ToString();

            await this.dbContext.Users.AddAsync(new ApplicationUser
            {
                UserName = "Momchil",
            });

            await this.dbContext.Trips.AddAsync(new Trip
            {
                Id = tripId,
                FromDestinationName = "Silistra",
                ToDestinationName = "Sofia",
            });

            await this.dbContext.SaveChangesAsync();

            var editModel = new TripEditInputModel
            {
                Id = tripId,
                FromDestinationName = "Varna",
                ToDestinationName = "Burgas",
            };

            await this.tripsService.EditAsync(editModel);

            var result = await this.dbContext.Trips.FirstOrDefaultAsync();
            var exceptionResult = await this.dbContext.Trips.FirstOrDefaultAsync(x => x.Id == "nonExistenceId");

            Assert.NotNull(result);
            Assert.Equal("Varna", result.FromDestinationName);
            Assert.Equal("Burgas", result.ToDestinationName);
        }

        [Fact]
        public async Task EditAsyncShouldThrowNullReferenceIfTripDoesNotExist()
        {
            await this.InitializeAsync();

            var nonExistenceModel = new TripEditInputModel
            {
                Id = "nonExistenceId",
            };

            await Assert.ThrowsAsync<NullReferenceException>(() => this.tripsService.EditAsync(nonExistenceModel));
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            await this.InitializeAsync();

            await this.dbContext.Trips.AddAsync(new Trip { Id = "TripDeleteId" });
            await this.dbContext.SaveChangesAsync();

            await this.tripsService.DeleteAsync("TripDeleteId");

            var trip = await this.dbContext.Trips.FirstOrDefaultAsync(x => x.Id == "TripDeleteId");

            Assert.Null(trip);
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowNullReferenceIfTripDoesNotExist()
        {
            await this.InitializeAsync();

            await Assert.ThrowsAsync<NullReferenceException>(() => this.tripsService.DeleteAsync("NonExistenceId"));
        }

        [Fact]
        public async Task IsTripExistReturnsCorrectValue()
        {
            await this.InitializeAsync();

            var user = new ApplicationUser
            {
                UserName = "Momchil",
            };
            var trip = new Trip
            {
                Id = "ExistingId",
                User = user,
            };
            await this.dbContext.Trips.AddAsync(trip);
            await this.dbContext.Users.AddAsync(user);
            await this.dbContext.SaveChangesAsync();

            var trueResult = await this.tripsService.IsTripExist("ExistingId", user.UserName);
            var falseResult = await this.tripsService.IsTripExist("NonExistingId", user.UserName);

            Assert.True(trueResult);
            Assert.False(falseResult);
        }

        [Fact]
        public async Task GetByIdShouldReturnEntityInCorrectGenericType()
        {
            await this.InitializeAsync();

            await this.dbContext.Trips.AddAsync(new Trip
            {
                Id = "TestGetId",
            });
            await this.dbContext.SaveChangesAsync();

            this.InitializeMapper();

            var result = this.tripsService.GetById<TripEditInputModel>("TestGetId");
            var expected = await this.dbContext.Trips.FirstAsync(x => x.Id == "TestGetId");

            Assert.NotNull(result);
            Assert.IsType<TripEditInputModel>(result);
            Assert.Equal(expected.Id, result.Id);
        }

        [Fact]
        public async Task GetUserTripsWithUserUsernameShouldReturnCorrectly()
        {
            await this.InitializeAsync();

            var user = new ApplicationUser
            {
                Id = "UserTestId",
                UserName = "Momchil",
            };
            await this.dbContext.Trips.AddRangeAsync(
                new Trip
                {
                    Id = "TestGetId",
                    User = user,
                },
                new Trip
                {
                    Id = "TestGetId2",
                    User = user,
                });

            await this.dbContext.Users.AddAsync(user);
            await this.dbContext.SaveChangesAsync();
            this.InitializeMapper();

            var expectedUser = await this.dbContext.Users.FirstAsync(x => x.Id == "UserTestId");
            var actualResult = this.tripsService.GetUserTripsWithUsername<TripEditInputModel>(expectedUser.UserName);

            Assert.NotNull(actualResult);
            Assert.Equal(2, actualResult.Count());
            Assert.IsType<List<TripEditInputModel>>(actualResult);
        }

        [Fact]
        public async Task GetTripPostsShouldReturnCorrectEntities()
        {
            await this.InitializeAsync();

            var car = new Car
            {
                Id = 1,
                Brand = "BMW",
                IsDeleted = false,
            };

            await this.dbContext.Cars.AddAsync(car);
            await this.dbContext.Trips.AddRangeAsync(
                new Trip
                {
                    Id = "TestGetId",
                    Car = car,
                },
                new Trip
                {
                    Id = "TestGetId2",
                    Car = car,
                });
            await this.dbContext.SaveChangesAsync();

            this.InitializeMapper();

            var resultOne = this.tripsService.GetTripPosts<TripEditInputModel>(); // Take all
            var resultTwo = this.tripsService.GetTripPosts<TripEditInputModel>(1); // Take one

            Assert.NotNull(resultOne);
            Assert.NotNull(resultTwo);
            Assert.Equal(2, resultOne.Count());
            Assert.Single(resultTwo);
            Assert.Equal(this.dbContext.Trips.FirstOrDefault(x => x.Id == "TestGetId").Id, resultOne.FirstOrDefault(x => x.Id == "TestGetId").Id);
        }

        [Fact]
        public async Task CountShouldReturnCorrectValue()
        {
            await this.InitializeAsync();

            await this.dbContext.Trips.AddRangeAsync(
                                    new Trip(),
                                    new Trip()
                                );
            await this.dbContext.SaveChangesAsync();

            Assert.Equal(2, this.tripsService.Count());
        }

        [Fact]
        public async Task GetAllDestinationsAsyncShouldReturnCorrectValues()
        {
            await this.InitializeAsync();

            await this.dbContext.Destinations.AddRangeAsync(
                                    new Destination(),
                                    new Destination(),
                                    new Destination()
                                );
            await this.dbContext.SaveChangesAsync();

            this.InitializeMapper();

            var actualResult = await this.tripsService.GetAllDestinationsAsync<DestinationViewModel>();
            var expectedResult = this.dbContext.Destinations.Count();

            Assert.NotNull(actualResult);
            Assert.Equal(expectedResult, actualResult.Count());
        }

        private void InitializeMapper()
           => AutoMapperConfig.RegisterMappings(
               typeof(ErrorViewModel).GetTypeInfo().Assembly,
               typeof(TripDetailsViewModel).GetTypeInfo().Assembly,
               typeof(TripEditInputModel).GetTypeInfo().Assembly);
    }
}
