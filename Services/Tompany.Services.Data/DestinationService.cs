namespace Tompany.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Tompany.Data;
    using Tompany.Services.Data.Contracts;
    using Tompany.Services.Mapping;
    using Tompany.Web.ViewModels.Destinations.ViewModels;
    using Tompany.Web.ViewModels.Trips.ViewModels;

    public class DestinationService : IDestinationService
    {
        private readonly IUnitOfWork unitOfWork;

        public DestinationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<DestinationViewModel> GetAllDestinationsAsync()
        {
            var destinations = this.unitOfWork.Destinations
                .All()
                .To<DestinationViewModel>()
                .ToList();

            return destinations;
        }

        public async Task<TripSearchViewModel> GetSearchResultAsync(int fromDestinationId, int toDestinationId, DateTime? dateOfDeparture)
        {
            var fromDestination = await this.unitOfWork.Destinations.All().Where(x => x.Id == fromDestinationId).FirstOrDefaultAsync();
            var toDestination = await this.unitOfWork.Destinations.All().Where(x => x.Id == toDestinationId).FirstOrDefaultAsync();

            var trips = this.unitOfWork.Trips.All()
                .Where(x => x.FromDestinationName == fromDestination.Name && x.ToDestinationName == toDestination.Name)
                .To<TripDetailsViewModel>()
                .ToList();

            if (dateOfDeparture != null)
            {
                trips.Where(x => x.DateOfDeparture == dateOfDeparture);
            }

            var serachResultViewModel = new TripSearchViewModel
            {
                Trips = trips,
                DateOfDeparture = dateOfDeparture,
            };

            return serachResultViewModel;
        }
    }
}
