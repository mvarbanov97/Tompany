using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.ViewModels.WatchListTrips;

namespace Tompany.Services.Data
{
    public class WatchListTripsService : IWatchListsService
    {
        private readonly IDeletableEntityRepository<WatchListTrip> watchListRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly ITripsService tripsService;

        public WatchListTripsService(
            IDeletableEntityRepository<WatchListTrip> watchListRepository,
            IRepository<ApplicationUser> usersRepository,
            ITripsService tripsService)
        {
            this.watchListRepository = watchListRepository;
            this.usersRepository = usersRepository;
            this.tripsService = tripsService;
        }

        public async Task AddAsync(string id, string username)
        {
            var user = this.usersRepository.All().Include(x => x.WatchListTrips).FirstOrDefault(x => x.UserName == username);
            if (user == null || user.WatchListTrips.Any(x => x.TripId == id))
            {
                return;
            }

            var trip = this.tripsService.GetById(id);
            if (trip == null)
            {
                return;
            }

            if (this.watchListRepository.All().Any(x => x.UserId == user.Id && x.TripId == trip.Id))
            {
                return;
            }

            var watchListTrip = new WatchListTrip
            {
                UserId = user.Id,
                TripId = trip.Id,
            };

            user.WatchListTrips.Add(watchListTrip);
            this.usersRepository.Update(user);
            await this.usersRepository.SaveChangesAsync();

            return;
        }

        public IEnumerable<WatchListTripViewModel> All(string username)
        {

            var watchListTrips = this.watchListRepository.All().Include(x => x.Trip).ThenInclude(x => x.User)
                                                       .Where(x => x.User.UserName == username && x.IsDeleted == false).To<WatchListTripViewModel>().ToList();

            if (watchListTrips == null)
            {
                return new List<WatchListTripViewModel>();
            }

            return watchListTrips;
        }

        public async Task Delete(string id, string userName)
        {
            var watchListTrip = this.watchListRepository.All().FirstOrDefault(x => x.User.UserName == userName && x.Trip.Id == id);

            if (watchListTrip == null)
            {
                return;
            }

            watchListTrip.IsDeleted = true;
            watchListTrip.DeletedOn = DateTime.Now;

            this.watchListRepository.Update(watchListTrip);
            await this.watchListRepository.SaveChangesAsync();
        }
    }
}
