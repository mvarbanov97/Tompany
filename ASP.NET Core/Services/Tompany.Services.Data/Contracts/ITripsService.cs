namespace Tompany.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tompany.Web.ViewModels.Trips;

    public interface ITripsService
    {
        Task CreateAsync(TripCreateInputModel travelCreateInputModel, string userId);

        T GetById<T>(string id);

        IEnumerable<T> GetAll<T>(int? count = null);
    }
}
