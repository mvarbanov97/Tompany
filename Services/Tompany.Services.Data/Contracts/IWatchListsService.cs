using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.WatchListTrips;

namespace Tompany.Services.Data.Contracts
{
    public interface IWatchListsService
    {
        Task AddAsync(string id, string name);

        IEnumerable<WatchListTripViewModel> All(string name);

        Task Delete(string id, string name);
    }
}
