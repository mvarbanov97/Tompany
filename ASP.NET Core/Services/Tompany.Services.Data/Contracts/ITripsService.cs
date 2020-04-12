namespace Tompany.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Tompany.Web.ViewModels.Travels;

    public interface ITripsService
    {
        Task CreateAsync(TravelCreateInputModel travelCreateInputModel, string userId);
    }
}
