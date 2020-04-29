using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Trips;

namespace Tompany.Services.Data
{
    public class ViewsService : IViewService
    {
        private readonly IRepository<Trip> tripsRepository;
        private readonly ITripsService tripsService;
        private readonly IRepository<View> viewsRepository;

        public ViewsService(
            IRepository<Trip> tripsRepository,
            ITripsService tripsService,
            IRepository<View> viewsRepository
            )
        {
            this.tripsRepository = tripsRepository;
            this.tripsService = tripsService;
            this.viewsRepository = viewsRepository;
        }

        public async Task AddViewAsync(string tripId)
        {
            var trip = this.tripsService.GetById(tripId);

            var view = new View { UserId = trip.UserId , Trip = trip };
            trip.Views.Add(view);

            await this.viewsRepository.AddAsync(view);
            await this.viewsRepository.SaveChangesAsync();
        }
    }
}
