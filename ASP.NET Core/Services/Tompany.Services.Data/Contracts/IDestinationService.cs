using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.Destinations.ViewModels;
using Tompany.Web.ViewModels.Trips.ViewModels;

namespace Tompany.Services.Data.Contracts
{
    public interface IDestinationService
    {
        IEnumerable<DestinationViewModel> GetAllDestinationsAsync();

        Task<TripSearchResultViewModel> GetSearchResultAsync(int fromDestinationId, int toDestinationId, DateTime? dateOfDeparture);
    }
}
