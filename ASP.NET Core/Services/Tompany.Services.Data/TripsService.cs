using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Services.Data
{
    public class TripsService : ITripsService
    {
        private readonly IRepository<Trip> tripsRepository;
        private readonly ILogger<TripsService> logger;

        public TripsService(
            IRepository<Trip> tripsRepository,
            ILogger<TripsService> logger)
        {
            this.tripsRepository = tripsRepository;
            this.logger = logger;
        }

        public async Task CreateAsync(TripCreateInputModel input, string userId)
        {
            var travel = new Trip
            {
                UserId = userId,
                CarId = input.CarId,
                PricePerPassenger = input.PricePerPassenger,
                FromCity = input.FromCity,
                ToCity = input.ToCity,
                DateOfDeparture = input.DateOfDeparture,
                TimeOfDeparture = input.TimeOfDeparture,
                AdditionalInformation = input.AdditionalInformation,
            };

            await this.tripsRepository.AddAsync(travel);
            await this.tripsRepository.SaveChangesAsync();
        }

        public T GetById<T>(string id)
        {
            var trip = this.tripsRepository.All().Where(x => x.Id == id)
                .To<T>().FirstOrDefault();

            return trip;
        }

        public IEnumerable<T> GetAll<T>(int? count = null)
        {
            IQueryable<Trip> query = this.tripsRepository.All().Where(x => x.Car.IsDeleted == false & x.IsDeleted == false);

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }

        public IEnumerable<T> GetTripPosts<T>(int? take = null, int skip = 0)
        {
            var query = this.tripsRepository
                .All()
                .Where(x => x.Car.IsDeleted == false & x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.To<T>().ToList();
        }

        public int GetTripsCount()
        {
            return this.tripsRepository.All().Count();
        }

        public async Task DeleteById(string id)
        {
            var tripToDelete = this.tripsRepository.All().FirstOrDefault(t => t.Id == id);

            if (tripToDelete == null)
            {
                throw new NullReferenceException($"Activity with id {id} not found.");
            }

            tripToDelete.IsDeleted = true;
            tripToDelete.DeletedOn = DateTime.Now;
            this.tripsRepository.Update(tripToDelete);
            await this.tripsRepository.SaveChangesAsync();
        }

        public async Task EditAsync(TripEditViewModel tripToEdit)
        {
            var trip = this.tripsRepository.All().FirstOrDefault(t => t.Id == tripToEdit.Id);

            if (trip == null)
            {
                throw new NullReferenceException($"Activity with id {tripToEdit.Id} not found");
            }

            trip.FromCity = tripToEdit.FromCity;
            trip.ToCity = tripToEdit.ToCity;
            trip.DateOfDeparture = tripToEdit.DateOfDeparture;
            trip.AdditionalInformation = tripToEdit.AdditionalInformation;

            this.tripsRepository.Update(trip);
            await this.tripsRepository.SaveChangesAsync();
        }

    }
}
