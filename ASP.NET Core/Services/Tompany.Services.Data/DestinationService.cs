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
using Tompany.Web.ViewModels.Destinations.ViewModels;
using Tompany.Web.ViewModels.Trips.ViewModels;
using Tompany.Web.ViewModels.Trips.InputModels;

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

        public async Task<TripSearchResultViewModel> GetSearchResultAsync(int fromDestinationId, int toDestinationId, DateTime? dateOfDeparture)
        {
            var fromDestination = await this.destinationsRepository.All().Where(x => x.Id == fromDestinationId).FirstOrDefaultAsync();
            var toDestination = await this.destinationsRepository.All().Where(x => x.Id == toDestinationId).FirstOrDefaultAsync();

            var trips = this.tripsRepository.All()
                .Where(x => x.FromDestinationName == fromDestination.Name && x.ToDestinationName == toDestination.Name)
                .To<TripDetailsViewModel>()
                .ToList();

            if (dateOfDeparture != null)
            {
                trips.Where(x => x.DateOfDeparture == dateOfDeparture);
            }

            var serachResultViewModel = new TripSearchResultViewModel
            {
                Trips = trips,
                DateOfDeparture = dateOfDeparture,
            };

            return serachResultViewModel;
        }
    }
}
