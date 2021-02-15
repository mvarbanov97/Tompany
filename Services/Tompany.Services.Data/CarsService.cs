namespace Tompany.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Tompany.Common;
    using Tompany.Data;
    using Tompany.Data.Models;
    using Tompany.Services.Data.Contracts;
    using Tompany.Services.Mapping;
    using Tompany.Web.ViewModels.Cars.InputModels;

    public class CarsService : ICarsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICloudinaryService cloudinaryService;

        public CarsService(
            ICloudinaryService cloudinary, IUnitOfWork unitOfWork)
        {
            this.cloudinaryService = cloudinary;
            this.unitOfWork = unitOfWork;
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

            await this.unitOfWork.Cars.AddAsync(car);
            await this.unitOfWork.CompleteAsync();
        }

        public async Task EditAsync(CarEditIputModel model, string userId)
        {
            var car = this.unitOfWork.Cars.All().Where(x => x.Id == model.Id && x.UserId == userId).FirstOrDefault();

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

            this.unitOfWork.Cars.Update(car);
            await this.unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var car = await this.unitOfWork.Cars
                .All()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (car == null)
            {
                throw new NullReferenceException($"Car with id {id} cannot be found.");
            }

            car.IsDeleted = true;
            this.unitOfWork.Cars.Update(car);
            await this.unitOfWork.CompleteAsync();
        }

        public T GetCarById<T>(int carId)
        {
            var car = this.unitOfWork.Cars
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
            var cars = this.unitOfWork.Cars
                .All()
                .Where(x => x.UserId == userId)
                .To<T>()
                .ToList();

            return cars;
        }

        public IEnumerable<T> GetAllUserCarsByUserUsername<T>(string username)
        {
            IQueryable<Car> query = this.unitOfWork.Cars
                .All()
                .Where(x => x.User.UserName == username);

            return query.To<T>().ToList();
        }

        public bool IsCarExists(int id, string userId)
        {
            var car = this.unitOfWork.Cars
                .All()
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
            var car = await this.unitOfWork.Cars
                .All()
                .Where(x => x.Id == id && x.UserId == userId)
                .FirstOrDefaultAsync();

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
