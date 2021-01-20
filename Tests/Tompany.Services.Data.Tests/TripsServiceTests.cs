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

namespace Tompany.Services.Data.Tests
{
    public class TripsServiceTests
    {
        [Fact]
        public async Task CreateAsyncShouldCreateCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "CreateTripAsyncDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);

            await dbContext.Users.AddAsync(new ApplicationUser
            {
                UserName = "Momchil",
            });

            await dbContext.SaveChangesAsync();

            await service.CreateAsync(new TripCreateInputModel
            {
                ApplicationUser = await dbContext.Users.FirstAsync(),
                FromDestinationName = "Silistra",
                ToDestinationName = "Sofia",
            });

            var expectedTrip = new Trip { FromDestinationName = "Silistra", ToDestinationName = "Sofia" };

            var actualTrip = await dbContext.Trips.FirstOrDefaultAsync(x => x.FromDestinationName == "Silistra" && x.ToDestinationName == "Sofia");

            Assert.NotNull(actualTrip);
            Assert.Equal(expectedTrip.ToDestinationName, actualTrip.ToDestinationName);
            Assert.Equal(expectedTrip.FromDestinationName, actualTrip.FromDestinationName);
        }

        [Fact]
        public async Task EditAsyncShouldEditCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "EditTripAsyncDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var tripId = Guid.NewGuid().ToString();

            await dbContext.Users.AddAsync(new ApplicationUser
            {
                UserName = "Momchil",
            });

            await dbContext.Trips.AddAsync(new Trip
            {
                Id = tripId,
                FromDestinationName = "Silistra",
                ToDestinationName = "Sofia",
            });

            await dbContext.SaveChangesAsync();

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);

            var editModel = new TripEditInputModel
            {
                Id = tripId,
                FromDestinationName = "Varna",
                ToDestinationName = "Burgas",
            };

            await service.EditAsync(editModel);

            var result = await dbContext.Trips.FirstOrDefaultAsync();
            var exceptionResult = await dbContext.Trips.FirstOrDefaultAsync(x => x.Id == "nonExistenceId");

            Assert.NotNull(result);
            Assert.Equal("Varna", result.FromDestinationName);
            Assert.Equal("Burgas", result.ToDestinationName);
        }

        [Fact]
        public async Task EditAsyncShouldThrowNullReferenceIfTripDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "EditTripAsyncThrowNullDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);

            var nonExistenceModel = new TripEditInputModel
            {
                Id = "nonExistenceId",
            };

            await Assert.ThrowsAsync<NullReferenceException>(() => service.EditAsync(nonExistenceModel));
        }

        [Fact]
        public async Task DeleteAsyncShouldDeleteCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "DeleteAsyncDb").Options;
            var dbContext = new ApplicationDbContext(options);

            await dbContext.Trips.AddAsync(new Trip { Id = "TripDeleteId" });
            await dbContext.SaveChangesAsync();

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);

            await service.DeleteAsync("TripDeleteId");

            var trip = await dbContext.Trips.FirstOrDefaultAsync(x => x.Id == "TripDeleteId");

            Assert.Null(trip);
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowNullReferenceIfTripDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "DeleteTripAsyncThrowNullDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);

            await Assert.ThrowsAsync<NullReferenceException>(() => service.DeleteAsync("NonExistenceId"));
        }

        [Fact]
        public async Task IsTripExistReturnsCorrectValue()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "IsExistDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var user = new ApplicationUser
            {
                UserName = "Momchil",
            };
            var trip = new Trip
            {
                Id = "ExistingId",
                User = user,
            };
            await dbContext.Trips.AddAsync(trip);
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);

            var trueResult = await service.IsTripExist("ExistingId", user.UserName);
            var falseResult = await service.IsTripExist("NonExistingId", user.UserName);

            Assert.True(trueResult);
            Assert.False(falseResult);
        }

        [Fact]
        public async Task GetByIdShouldReturnEntityInCorrectGenericType()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "GetTripByIdDb").Options;
            var dbContext = new ApplicationDbContext(options);

            await dbContext.Trips.AddAsync(new Trip
            {
                Id = "TestGetId",
            });
            await dbContext.SaveChangesAsync();

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);
            this.InitializeMapper();

            var result = service.GetById<TripEditInputModel>("TestGetId");
            var expected = await dbContext.Trips.FirstAsync(x => x.Id == "TestGetId");

            Assert.NotNull(result);
            Assert.IsType<TripEditInputModel>(result);
            Assert.Equal(expected.Id, result.Id);
        }

        [Fact]
        public async Task GetUserTripsWithUserUsernameShouldReturnCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "GetUserTripsWithUserUsernameDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var user = new ApplicationUser
            {
                Id = "UserTestId",
                UserName = "Momchil",
            };
            await dbContext.Trips.AddRangeAsync(
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

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);
            this.InitializeMapper();

            var expectedUser = await dbContext.Users.FirstAsync(x => x.Id == "UserTestId");
            var actualResult = service.GetUserTripsWithUsername<TripEditInputModel>(expectedUser.UserName);

            Assert.NotNull(actualResult);
            Assert.Equal(2, actualResult.Count());
            Assert.IsType<List<TripEditInputModel>>(actualResult);
        }

        [Fact]
        public async Task GetTripPostsShouldReturnCorrectEntities()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "GetTripPostseDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var car = new Car
            {
                Id = 1,
                Brand = "BMW",
                IsDeleted = false,
            };

            await dbContext.Cars.AddAsync(car);
            await dbContext.Trips.AddRangeAsync(
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
            await dbContext.SaveChangesAsync();

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);
            this.InitializeMapper();

            var resultOne = service.GetTripPosts<TripEditInputModel>(); // Take all
            var resultTwo = service.GetTripPosts<TripEditInputModel>(1); // Take one

            Assert.NotNull(resultOne);
            Assert.NotNull(resultTwo);
            Assert.Equal(2, resultOne.Count());
            Assert.Single(resultTwo);
            Assert.Equal(dbContext.Trips.FirstOrDefault(x => x.Id == "TestGetId").Id, resultOne.FirstOrDefault(x => x.Id == "TestGetId").Id);
        }

        [Fact]
        public async Task CountShouldReturnCorrectValue()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "GetUserTripsWithUserUsernameDb").Options;
            var dbContext = new ApplicationDbContext(options);


            await dbContext.Trips.AddRangeAsync(
                                    new Trip(),
                                    new Trip()
                                );
            await dbContext.SaveChangesAsync();

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);

            Assert.Equal(2, service.Count());
        }

        [Fact]
        public async Task GetAllDestinationsAsyncShouldReturnCorrectValues()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "GetAllDestinationsDb").Options;
            var dbContext = new ApplicationDbContext(options);


            await dbContext.Destinations.AddRangeAsync(
                                    new Destination(),
                                    new Destination(),
                                    new Destination()
                                );
            await dbContext.SaveChangesAsync();

            var tripsRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);

            var service = new TripsService(tripsRepository, destinationRepository, usersRepository);
            this.InitializeMapper();

            var actualResult = await service.GetAllDestinationsAsync<DestinationViewModel>();
            var expectedResult = dbContext.Destinations.Count();

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
