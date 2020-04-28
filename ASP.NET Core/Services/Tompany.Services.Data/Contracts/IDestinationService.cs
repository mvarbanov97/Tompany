using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.Destinations;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Services.Data.Contracts
{
    public interface IDestinationService
    {
        Task<IEnumerable<DestinationViewModel>> GetAllDestinationsAsync();

        Task<TripSearchResultViewModel> GetSearchResultAsync(int fromDestinationId, int toDestinationId, DateTime dateOfDeparture);
    }
}
