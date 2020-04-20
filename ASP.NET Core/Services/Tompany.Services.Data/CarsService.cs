using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Cars;

namespace Tompany.Services.Data
{
    public class CarsService : ICarsService
    {
        private readonly IRepository<Car> carsRepository;

        public CarsService(
            IRepository<Car> carsRepository)
        {
            this.carsRepository = carsRepository;
        }

        public IEnumerable<T> GetCarByUserId<T>(string userId)
        {
            IQueryable<Car> query = this.carsRepository.All().Where(x => x.UserId == userId);

            return query.To<T>().ToList();
        }

        public T GetById<T>(int carId)
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

        public IEnumerable<T> GetAll<T>(int? count = null)
        {
            IQueryable<Car> query = this.carsRepository.All();

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public async Task CreateAsync(string userId, CarCreateInputModel carInputModel)
        {
            var car = new Car
            {
                UserId = userId,
                ImageUrl = carInputModel.ImageUrl,
                Brand = carInputModel.Brand,
                Model = carInputModel.Model,
                YearOfManufacture = carInputModel.YearOfManufacture,
                Color = carInputModel.Color,
                Seats = carInputModel.Seats,
                IsLuggageAvaliable = carInputModel.IsLuggageAvaliable,
                IsSmokingAllowed = carInputModel.IsSmokingAllowed,
                IsAirConditiningAvailable = carInputModel.IsAirConditiningAvailable,
                IsAllowedForPets = carInputModel.IsAllowedForPets,
            };

            await this.carsRepository.AddAsync(car);
            await this.carsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var car = this.carsRepository.All().FirstOrDefault(x => x.Id == id);

            if (car == null)
            {
                throw new NullReferenceException($"Car with id {id} cannot be found.");
            }

            car.IsDeleted = true;
            this.carsRepository.Update(car);
            await this.carsRepository.SaveChangesAsync();
        }
    }
}
