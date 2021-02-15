namespace Tompany.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Tompany.Data;
    using Tompany.Data.Models;
    using Tompany.Services.Data.Contracts;
    using Tompany.Services.Mapping;
    using Tompany.Web.ViewModels.Trips.InputModels;

    public class TripsService : ITripsService
    {
        private readonly IUnitOfWork unitOfWork;

        public TripsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(TripCreateInputModel input)
        {
            var trip = new Trip
            {
                FromDestinationName = input.FromDestinationName,
                ToDestinationName = input.ToDestinationName,
                User = input.ApplicationUser,
                UserId = input.UserId,
                Car = input.Car,
                CarId = input.CarId,
                PricePerPassenger = input.PricePerPassenger,
                DateOfDeparture = input.DateOfDeparture,
                TimeOfDeparture = input.TimeOfDeparture,
                AdditionalInformation = input.AdditionalInformation,
                GroupName = Guid.NewGuid().ToString(),
            };

            var userTrip = new UserTrip
            {
                User = trip.User,
                UserId = trip.UserId,
                Trip = trip,
                TripId = trip.Id,
            };

            var user = input.ApplicationUser;
            user.UserTrips.Add(userTrip);
            this.unitOfWork.Users.Update(user);
            await this.unitOfWork.CompleteAsync();
        }

        public async Task EditAsync(TripEditInputModel tripToEdit)
        {
            var trip = this.unitOfWork.Trips.All().FirstOrDefault(t => t.Id == tripToEdit.Id);

            if (trip == null)
            {
                throw new NullReferenceException($"Activity with id {tripToEdit.Id} not found");
            }

            trip.FromDestinationName = tripToEdit.FromDestinationName;
            trip.ToDestinationName = tripToEdit.ToDestinationName;
            trip.DateOfDeparture = tripToEdit.DateOfDeparture;
            trip.AdditionalInformation = tripToEdit.AdditionalInformation;
            trip.CarId = tripToEdit.CarId;

            this.unitOfWork.Trips.Update(trip);
            await this.unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var tripToDelete = this.unitOfWork.Trips.All().FirstOrDefault(t => t.Id == id);

            if (tripToDelete == null)
            {
                throw new NullReferenceException($"Activity with id {id} not found.");
            }

            tripToDelete.IsDeleted = true;
            tripToDelete.DeletedOn = DateTime.Now;
            this.unitOfWork.Trips.Update(tripToDelete);
            await this.unitOfWork.CompleteAsync();
        }

        public async Task<bool> IsTripExist(string id, string username)
        {
            var trip = await this.unitOfWork.Trips.All().AnyAsync(x => x.Id == id && x.User.UserName == username);

            return trip;
        }

        public T GetById<T>(string id)
        {
            var trip = this.unitOfWork.Trips
                .All()
                .Include(x => x.TripRequest)
                .Include(x => x.Car)
                .Include(x => x.Views)
                .Include(x => x.Passengers)
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefault();

            return trip;
        }

        public Trip GetById(string id)
        {
            var trip = this.unitOfWork.Trips
                .All()
                .Include(x => x.Car)
                .Include(x => x.Passengers)
                .Include(x => x.TripRequest)
                .Include(x => x.Views)
                .Where(x => x.Id == id).FirstOrDefault();

            return trip;
        }

        public IEnumerable<T> GetUserTripsWithUsername<T>(string username)
        {
            var cars = this.unitOfWork.Trips
                .All()
                .Include(x => x.Passengers)
                .Include(x => x.Car)
                .Include(x => x.UserTrips)
                .Where(x => x.User.UserName == username)
                .To<T>()
                .ToList();

            return cars;
        }

        public IEnumerable<T> GetTripPosts<T>(int? take = null, int skip = 0)
        {
            var tripPosts = this.unitOfWork.Trips
                .All()
                .Where(x => x.Car.IsDeleted == false & x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip)
                .To<T>()
                .ToList();

            if (take.HasValue)
            {
                tripPosts = tripPosts.Take(take.Value).ToList();
            }

            return tripPosts;
        }

        public int Count()
        {
            return this.unitOfWork.Trips.All().Count();
        }

        public async Task<IEnumerable<T>> GetAllDestinationsAsync<T>()
        {
            IQueryable<Destination> query = this.unitOfWork.Destinations.All();

            return await query.To<T>().ToListAsync();
        }
    }
}
