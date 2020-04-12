namespace Tompany.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Tompany.Web.ViewModels.Travels;

    public interface ITravelsService
    {
        Task CreateAsync(TravelCreateInputModel travelCreateInputModel);
    }
}
