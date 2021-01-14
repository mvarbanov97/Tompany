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
        public CarsServiceTests()
        {
            this.InitializeMapper();
        }

        [Fact]
        public async Task CreateAsyncAddEntitySuccessfullyToDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CreateCarsDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Car>(dbContext);

            this.InitializeMapper();
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

            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);

            await service.CreateAsync(Guid.NewGuid().ToString(), car);

            Assert.Equal(1, await dbContext.Cars.CountAsync());
        }

        [Fact]
        public async Task EditAsyncShouldChangeEntityProperties()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "EditCarsDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);

            var userId = Guid.NewGuid().ToString();

            await dbContext
                .AddAsync(new Car
                {
                    Id = 1,
                    UserId = userId,
                    Brand = "Lada",
                    Model = "Niva",
                });
            await dbContext.SaveChangesAsync();

            var carToEdit = new CarEditIputModel
            {
                Id = 1,
                Brand = "BMW",
                Model = "320",
            };

            // Act
            await service.EditAsync(carToEdit, userId);
            var actualCar = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == 1);

            // Assert
            Assert.Equal("BMW", actualCar.Brand);
        }

        [Fact]
        public async Task DeleteAsyncRemoveEntityFromDbSuccessfully()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteCarsDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;
            var service = new CarsService(repository, mockCloudinary);

            var userId = Guid.NewGuid().ToString();

            await dbContext.AddAsync(new Car
            {
                Id = 1,
                UserId = userId,
                Brand = "CarBrandToDelete",
                Model = "CarModelToDelete",
            });

            await dbContext.SaveChangesAsync();

            await service.DeleteAsync(1, userId);

            var expectedResult = 0;
            var actualResult = await dbContext.Cars.CountAsync();
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async Task DeleteAsyncShouldThrowNullReferenceIfCarDoesNotExistInDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteCarsThatDoesNotExistInDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);
            var userId = Guid.NewGuid().ToString();

            await Assert.ThrowsAsync<NullReferenceException>(() => service.DeleteAsync(1, userId));
        }

        [Fact]
        public async Task GetCarByIdReturnEntityWhenElementExistWithPassedId()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CarsGetByIdDb").Options;
            var dbContext = new ApplicationDbContext(options);

            await dbContext.Cars.AddAsync(new Car
            {
                Id = 1,
            });

            await dbContext.SaveChangesAsync();

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);

            this.InitializeMapper();

            var result = service.GetCarById<CarViewModel>(1);

            Assert.Equal(1, result.Id);
            Assert.IsType<CarViewModel>(result);
        }

        [Fact]
        public void GetCarByIdShouldThrowNullReferenceWhenThereIsNoEntityWithPassedId()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetUnexistCarDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);
            this.InitializeMapper();

            var actualExMessage = $"Car with id 1 not found.";

            Assert.Throws<NullReferenceException>(() => service.GetCarById<CarViewModel>(1));
        }

        [Fact]
        public async Task GetUserCarsByIdShouldReturnEntitiesSuccessfully()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetUserCarsByIdDb").Options;
            var dbContext = new ApplicationDbContext(options);

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

            await dbContext.AddRangeAsync(cars);
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);
            this.InitializeMapper();

            var userCars = service.GetAllUserCarsByUserId<CarViewModel>(userId);

            Assert.Equal(2, userCars.Count());
        }

        [Fact]
        public async Task GetUserCarsByUsernameShouldReturnEntitiesSuccessfully()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GetUserCarsByUsernameDb").Options;
            var dbContext = new ApplicationDbContext(options);

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

            await dbContext.AddRangeAsync(cars);
            await dbContext.SaveChangesAsync();

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);
            this.InitializeMapper();

            var userCars = service.GetAllUserCarsByUserUsername<CarViewModel>(user.UserName);

            Assert.Equal(2, userCars.Count());
        }

        [Fact]
        public async Task IsCarExistShouldReturnCorrectValue()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "IsCarExistDb").Options;
            var dbContext = new ApplicationDbContext(options);

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

            await dbContext.AddRangeAsync(cars);
            await dbContext.SaveChangesAsync();

            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);

            var result = service.IsCarExists(2, user.Id);
            var secondResult = service.IsCarExists(3, user.Id);

            Assert.True(result);
            Assert.False(secondResult);
        }

        [Fact]
        public async Task ExtractCarShouldReturnValidEntity()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ExtractCarDb").Options;
            var dbContext = new ApplicationDbContext(options);
            var repository = new EfDeletableEntityRepository<Car>(dbContext);
            var mockCloudinary = new Mock<ICloudinaryService>().Object;

            var service = new CarsService(repository, mockCloudinary);

            var userId = Guid.NewGuid().ToString();
            this.InitializeMapper();

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

            await dbContext.Cars.AddAsync(car);
            await dbContext.SaveChangesAsync();

            var actual = 0;
            var extractedCar = await service.ExtractCar(1, userId);
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
