using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Microsoft.EntityFrameworkCore;
using Tompany.Data.Models.Enums;
using System.Threading.Tasks;
using Tompany.Services.Data.Contracts;

namespace Tompany.Services.Data
{
    public class TripRequestsService : ITripRequestsService
    {
        private readonly IRepository<TripRequest> tripRequestsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly ITripsService tripsService;
        private readonly IRepository<Trip> tripsRepository;

        public TripRequestsService(
            IRepository<TripRequest> tripRequestsRepository,
            IRepository<ApplicationUser> usersRepository,
            ITripsService tripsService,
            IRepository<Trip> tripsRepository)
        {
            this.tripRequestsRepository = tripRequestsRepository;
            this.usersRepository = usersRepository;
            this.tripsService = tripsService;
            this.tripsRepository = tripsRepository;
        }

        public TripRequest GetById(string id)
        {
            return this.tripRequestsRepository.All()
                                              .Where(x => x.Id == id)
                                              .FirstOrDefault();
        }

        public IEnumerable<TripRequest> GetAllTripRequestsByTripId(string tripId)
        {
            return this.tripRequestsRepository.All()
                                              .Include(x => x.Sender)
                                              .Where(x => x.TripId == tripId);
        }

        public IEnumerable<TripRequest> GetPendingRequestsByTripId(string tripId)
        {
            return this.tripRequestsRepository.All()
                                              .Include(x => x.Sender)
                                              .Include(x => x.Trip)
                                              .Where(x => x.TripId == tripId && x.RequestStatus == RequestStatus.Pending); 
        }

        public async Task SendTripRequest(string userName, string tripId, string ownerId)
        {
            ApplicationUser owner = null;
            owner = this.usersRepository.All().Include(x => x.UserTrips).FirstOrDefault(y => y.Id == ownerId);

            var trip = this.tripsService.GetById(tripId);

            ApplicationUser sender = null;
            sender = this.usersRepository.All().FirstOrDefault(x => x.UserName == userName);

            var tripRequest = new TripRequest
            {
                Trip = trip,
                TripId = tripId,
                Sender = sender,
                SenderId = sender.Id,
            };

            trip.TripRequest.Add(tripRequest);

            await this.tripRequestsRepository.AddAsync(tripRequest);
            await this.tripRequestsRepository.SaveChangesAsync();
        }
    }
}
