namespace Tompany.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tompany.Data.Models;
    using Tompany.Web.ViewModels.Trips;

    public interface ITripsService
    {
        Task CreateAsync(TripCreateInputModel travelCreateInputModel, string userId);

        T GetById<T>(string id);

        Trip GetById(string id);

        int GetTripsCount();

        Trip GetTripByUserId(string userId);

        IEnumerable<T> GetAll<T>(int? count = null);

        IEnumerable<T> GetUserTrips<T>(string userId);

        IEnumerable<T> GetTripPosts<T>(int? take = null, int skip = 0);

        Task<TripSearchViewModel> GetSearchResultAsync(int fromDestinationId, int toDestination, DateTime dateOfDeparture);

        Task DeleteById(string id);

        Task EditAsync(TripEditViewModel tripToEdit);
    }
}
