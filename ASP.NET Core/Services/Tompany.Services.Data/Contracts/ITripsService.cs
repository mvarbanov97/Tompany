﻿namespace Tompany.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tompany.Web.ViewModels.Trips;

    public interface ITripsService
    {
        Task CreateAsync(TripCreateInputModel travelCreateInputModel, string userId);

        T GetById<T>(string id);

        int GetTripsCount();

        T GetTripByUserId<T>(string userId, string id);

        IEnumerable<T> GetAll<T>(int? count = null);

        IEnumerable<T> GetUserTrips<T>(string userId);

        IEnumerable<T> GetTripPosts<T>(int? take = null, int skip = 0);

        Task SendTripRequest(string userId, string tripId, string ownerId);

        Task DeleteById(string id);

        Task EditAsync(TripEditViewModel tripToEdit);
    }
}
