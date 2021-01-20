namespace Tompany.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tompany.Data.Models;
    using Tompany.Web.ViewModels.Destinations;
    using Tompany.Web.ViewModels.Trips.InputModels;

    public interface ITripsService
    {
        Task CreateAsync(TripCreateInputModel travelCreateInputModel);

        Task EditAsync(TripEditInputModel tripToEdit);

        Task DeleteAsync(string id);

        Task<bool> IsTripExist(string id, string username);

        Trip GetById(string id);

        T GetById<T>(string id);

        int Count();

        IEnumerable<T> GetUserTripsWithUsername<T>(string username);

        IEnumerable<T> GetTripPosts<T>(int? take = null, int skip = 0);

        Task<IEnumerable<T>> GetAllDestinationsAsync<T>();
    }
}
