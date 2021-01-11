using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tompany.Data.Models;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Data;
using Tompany.Web.ViewModels.Cars.InputModels;
using Tompany.Web.ViewModels.Cars.ViewModels;
using Tompany.Services.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Tompany.Common;

namespace Tompany.Services.Data
{
    public class CarsService : ICarsService
    {
        private readonly IDeletableEntityRepository<Car> carsRepository;
        private readonly ICloudinaryService cloudinaryService;

        public CarsService(
            IDeletableEntityRepository<Car> carsRepository,
            ICloudinaryService cloudinary)
        {
            this.carsRepository = carsRepository;
            this.cloudinaryService = cloudinary;
        }

        public async Task CreateAsync(string userId, CarCreateInputModel input)
        {
            var car = new Car
            {
                UserId = userId,
                CarImageUrl = input.CarImageUrl,
                Brand = input.Brand,
                Model = input.Model,
                YearOfManufacture = input.YearOfManufacture,
                Color = input.Color,
                Seats = input.Seats,
                IsLuggageAvaliable = input.IsLuggageAvaliable,
                IsSmokingAllowed = input.IsSmokingAllowed,
                IsAirConditiningAvailable = input.IsAirConditiningAvailable,
                IsAllowedForPets = input.IsAllowedForPets,
            };

            if (input.CarPicture != null)
            {
                var carImageUrl = await this.cloudinaryService.UploadImageAsync(
                    input.CarPicture,
                    string.Format(GlobalConstants.CloudinaryCarPictureName, userId));
                car.CarImageUrl = carImageUrl;
            }

            await this.carsRepository.AddAsync(car);
            await this.carsRepository.SaveChangesAsync();
        }

        public async Task EditAsync(CarEditIputModel model, string userId)
        {
            var car = this.carsRepository.All().Where(x => x.Id == model.Id && x.UserId == userId).FirstOrDefault();

            car.CarImageUrl = model.CarImageUrl;
            car.Brand = model.Brand;
            car.Model = model.Model;
            car.YearOfManufacture = model.YearOfManufacture;
            car.Color = model.Color;
            car.Seats = model.Seats;
            car.IsLuggageAvaliable = model.IsLuggageAvaliable;
            car.IsSmokingAllowed = model.IsSmokingAllowed;
            car.IsAirConditiningAvailable = model.IsAirConditiningAvailable;
            car.IsAllowedForPets = model.IsAllowedForPets;

            this.carsRepository.Update(car);
            await this.carsRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var car = await this.carsRepository
                .All()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (car == null)
            {
                throw new NullReferenceException($"Car with id {id} cannot be found.");
            }

            car.IsDeleted = true;
            this.carsRepository.Update(car);
            await this.carsRepository.SaveChangesAsync();
        }

        public T GetCarById<T>(int carId)
        {
            var car = this.carsRepository
                .All()
                .Where(x => x.Id == carId)
                .To<T>()
                .FirstOrDefault();

            if (car == null)
            {
                throw new NullReferenceException($"Car with id {carId} not found.");
            }

            return car;
        }

        public IEnumerable<T> GetAllUserCarsByUserId<T>(string userId)
        {
            var cars = this.carsRepository.All().Where(x => x.UserId == userId).To<T>().ToList();

            return cars;
        }

        public IEnumerable<T> GetAllUserCarsByUserUsername<T>(string username)
        {
            IQueryable<Car> query = this.carsRepository.All().Where(x => x.User.UserName == username);

            return query.To<T>().ToList();
        }

        public bool IsCarExists(int id, string userId)
        {
            var car = this.carsRepository.All()
                .Include(x => x)
                .Where(x => x.Id == id && x.UserId == userId)
                .FirstOrDefault();

            if (car == null)
            {
                return false;
            }

            return true;
        }

        public async Task<CarEditIputModel> ExtractCar(int id, string userId)
        {
            var car = await this.carsRepository.All().Where(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();

            return new CarEditIputModel
            {
                Id = car.Id,
                UserId = userId,
                CarImageUrl = car.CarImageUrl,
                Brand = car.Brand,
                Model = car.Model,
                YearOfManufacture = car.YearOfManufacture,
                Color = car.Color,
                Seats = car.Seats,
                IsLuggageAvaliable = car.IsLuggageAvaliable,
                IsSmokingAllowed = car.IsSmokingAllowed,
                IsAirConditiningAvailable = car.IsAirConditiningAvailable,
                IsAllowedForPets = car.IsAllowedForPets,
            };
        }
    }
}
