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

        public TripsService(
            IRepository<Trip> tripsRepository)
        {
            this.tripsRepository = tripsRepository;
        }

        public async Task CreateAsync(TripCreateInputModel input, string userId)
        {
            var travel = new Trip
            {
                UserId = userId,
                CarId = input.CarId,
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
            IQueryable<Trip> query = this.tripsRepository.All();

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return query.To<T>().ToList();
        }
    }
}
