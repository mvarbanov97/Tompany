using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tompany.Common;
using Tompany.Data;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Data.Repositories;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels;
using Tompany.Web.ViewModels.Cars.InputModels;
using Tompany.Web.ViewModels.Cars.ViewModels;
using Xunit;

namespace Tompany.Services.Data.Tests
{
    public class CarsServiceTests
    {
        private CarsService carsService;
        private ApplicationDbContext dbContext;

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            this.dbContext = new ApplicationDbContext(options);
            var unitOfWork = new UnitOfWork(this.dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;
            this.carsService = new CarsService(mockCloudinary, unitOfWork);

            this.InitializeMapper();
        }

        [Fact]
        public async Task CreateAsyncAddEntitySuccessfullyToDb()
        {
            await this.InitializeAsync();

            var car = new CarCreateInputModel
            {
                CarImageUrl = GlobalConstants.NoCarPictureLocation,
                Brand = "TestBrand",
                Model = "TestModel",
                YearOfManufacture = 2010,
                Color = "testColor",
                Seats = 4,
                IsLuggageAvaliable = true,
                IsSmokingAllowed = true,
                IsAirConditiningAvailable = true,
                IsAllowedForPets = true,
            };

            await this.carsService.CreateAsync(Guid.NewGuid().ToString(), car);

            Assert.Equal(1, await this.dbContext.Cars.CountAsync());
        }

        [Fact]
        public async Task EditAsyncShouldChangeEntityProperties()
        {
            await this.InitializeAsync();

            var userId = Guid.NewGuid().ToString();

            await this.dbContext
                .AddAsync(new Car
                {
                    Id = 1,
                    UserId = userId,
                    Brand = "Lada",
                    Model = "Niva",
                });
            await this.dbContext.SaveChangesAsync();

            var carToEdit = new CarEditIputModel
            {
                Id = 1,
                Brand = "BMW",
                Model = "320",
            };

            // Act
            await this.carsService.EditAsync(carToEdit, userId);
            var actualCar = await this.dbContext.Cars.FirstOrDefaultAsync(x => x.Id == 1);

            // Assert
            Assert.Equal("BMW", actualCar.Brand);
        }

        [Fact]
        public async Task DeleteAsyncRemoveEntityFromDbSuccessfully()
        {
            await this.InitializeAsync();

            var userId = Guid.NewGuid().ToString();

            await this.dbContext.AddAsync(new Car
            {
                Id = 1,
                UserId = userId,
                Brand = "CarBrandToDelete",
                Model = "CarModelToDelete",
            });

            await this.dbContext.SaveChangesAsync();

            await this.carsService.DeleteAsync(1, userId);

            var expectedResult = 0;
            var actualResult = await this.dbContext.Cars.CountAsync();
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowNullReferenceIfCarDoesNotExistInDb()
        {
            await this.InitializeAsync();

            var userId = Guid.NewGuid().ToString();

            await Assert.ThrowsAsync<NullReferenceException>(() => this.carsService.DeleteAsync(1, userId));
        }

        [Fact]
        public async Task GetCarByIdReturnEntityWhenElementExistWithPassedId()
        {
            await this.InitializeAsync();

            await this.dbContext.Cars.AddAsync(new Car
            {
                Id = 1,
            });

            await this.dbContext.SaveChangesAsync();

            this.InitializeMapper();

            var result = this.carsService.GetCarById<CarViewModel>(1);

            Assert.Equal(1, result.Id);
            Assert.IsType<CarViewModel>(result);
        }

        [Fact]
        public async Task GetCarByIdShouldThrowNullReferenceWhenThereIsNoEntityWithPassedId()
        {
            await this.InitializeAsync();

            var actualExMessage = $"Car with id 1 not found.";

            Assert.Throws<NullReferenceException>(() => this.carsService.GetCarById<CarViewModel>(1));
        }

        [Fact]
        public async Task GetUserCarsByIdShouldReturnEntitiesSuccessfully()
        {
            await this.InitializeAsync();

            var userId = Guid.NewGuid().ToString();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "Momchil",
            };
            var cars = new List<Car>();
            cars.Add(new Car
            {
                Id = 1,
                User = user,
                UserId = userId,
                Brand = "BMW",
            });
            cars.Add(new Car
            {
                Id = 2,
                User = user,
                UserId = userId,
                Brand = "Honda",
            });

            await this.dbContext.AddRangeAsync(cars);
            await this.dbContext.AddAsync(user);
            await this.dbContext.SaveChangesAsync();

            var userCars = this.carsService.GetAllUserCarsByUserId<CarViewModel>(userId);

            Assert.Equal(2, userCars.Count());
        }

        [Fact]
        public async Task GetUserCarsByUsernameShouldReturnEntitiesSuccessfully()
        {
            await this.InitializeAsync();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUsername",
            };

            var cars = new List<Car>();
            cars.Add(new Car
            {
                Id = 1,
                User = user,
                Brand = "BMW",
            });
            cars.Add(new Car
            {
                Id = 2,
                User = user,
                Brand = "Honda",
            });

            await this.dbContext.AddRangeAsync(cars);
            await this.dbContext.SaveChangesAsync();

            var userCars = this.carsService.GetAllUserCarsByUserUsername<CarViewModel>(user.UserName);

            Assert.Equal(2, userCars.Count());
        }

        [Fact]
        public async Task IsCarExistShouldReturnCorrectValue()
        {
            await this.InitializeAsync();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUsername",
            };

            var cars = new List<Car>();
            cars.Add(new Car
            {
                Id = 1,
                User = user,
                Brand = "BMW",
            });
            cars.Add(new Car
            {
                Id = 2,
                User = user,
                Brand = "Honda",
            });

            await this.dbContext.AddRangeAsync(cars);
            await this.dbContext.SaveChangesAsync();

            var result = this.carsService.IsCarExists(2, user.Id);
            var secondResult = this.carsService.IsCarExists(3, user.Id);

            Assert.True(result);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task ExtractCarShouldReturnValidEntity()
        {
            await this.InitializeAsync();

            var userId = Guid.NewGuid().ToString();

            var car = new Car
            {
                Id = 1,
                UserId = userId,
                CarImageUrl = GlobalConstants.NoCarPictureLocation,
                Brand = "BMW",
                Model = "320",
                YearOfManufacture = 2010,
                Color = "white",
                Seats = 4,
                IsLuggageAvaliable = true,
                IsSmokingAllowed = true,
                IsAirConditiningAvailable = true,
                IsAllowedForPets = true,
            };

            await this.dbContext.Cars.AddAsync(car);
            await this.dbContext.SaveChangesAsync();

            var actual = 0;
            var extractedCar = await this.carsService.ExtractCar(1, userId);
            if (extractedCar != null)
            {
                actual = 1;
            }

            Assert.Equal(1, actual);
        }

        private void InitializeMapper()
           => AutoMapperConfig.RegisterMappings(
               typeof(ErrorViewModel).GetTypeInfo().Assembly,
               typeof(CarViewModel).GetTypeInfo().Assembly);
    }
}
