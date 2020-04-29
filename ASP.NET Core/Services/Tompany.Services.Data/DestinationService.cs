using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.Destinations;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Services.Data
{
    public class DestinationService : IDestinationService
    {
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<Trip> tripsRepository;


        public DestinationService(
            IRepository<Destination> destinationsRepository,
            IRepository<Trip> tripsRepository)
        {
            this.destinationsRepository = destinationsRepository;
            this.tripsRepository = tripsRepository;
        }

        public IEnumerable<DestinationViewModel> GetAllDestinationsAsync()
        {
            var destinations = this.destinationsRepository
                .All()
                .To<DestinationViewModel>()
                .ToArray();

            return destinations;
        }

        public async Task<IEnumerable<string>> GetAllDestinations()
        {
            var destinations = this.destinationsRepository
                .All()
                .Select(x => x.Name)
                .ToArray();

            return destinations;
        }

        public async Task<TripSearchResultViewModel> GetSearchResultAsync(int fromDestinationId, int toDestinationId, DateTime dateOfDeparture)
        {
            var destinations = this.destinationsRepository.All().ToArray();

            var fromDestination = this.destinationsRepository.All().Where(x => x.Id == fromDestinationId).FirstOrDefault();
            var toDestination = this.destinationsRepository.All().Where(x => x.Id == toDestinationId).FirstOrDefault();

            var trips = this.tripsRepository.All().Where(x => x.FromDestinationName == fromDestination.Name && x.ToDestinationName == toDestination.Name)
                .To<TripDetailsViewModel>()
                .ToList();

            var serachResultViewModel = new TripSearchResultViewModel
            {
                Trips = trips,
                DateOfDeparture = dateOfDeparture,
            };

            return serachResultViewModel;
        }
    }
}
