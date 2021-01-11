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
using Tompany.Web.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Tompany.Web.Infrastructure.Contracts;

namespace Tompany.Services.Data
{
    public class TripRequestsService : ITripRequestsService
    {
        private readonly IRepository<TripRequest> tripRequestsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;

        private readonly IHubContext<NotificationHub> hubContext;
        private readonly INotificationService notificationService;

        public TripRequestsService(
            IRepository<TripRequest> tripRequestsRepository,
            IRepository<ApplicationUser> usersRepository,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService)
        {
            this.tripRequestsRepository = tripRequestsRepository;
            this.usersRepository = usersRepository;
            this.hubContext = hubContext;
            this.notificationService = notificationService;
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

        public async Task<bool> SendTripRequest(string userName, Trip trip, string ownerId)
        {
            var isRequestSent = false;

            var owner = await this.usersRepository
                .All()
                .Where(x => x.Id == ownerId)
                .Include(x => x.UserTrips)
                .Include(x => x.TripRequests)
                .FirstOrDefaultAsync();

            var sender = await this.usersRepository
                .All()
                .Where(x => x.UserName == userName)
                .FirstOrDefaultAsync();

            if (await this.IsRequesAlreadySend(sender.Id, trip.Id))
            {
                return isRequestSent;
            }
            else
            {
                isRequestSent = true;

                var tripRequest = new TripRequest
                {
                    Trip = trip,
                    TripId = trip.Id,
                    Sender = sender,
                    SenderId = sender.Id,
                };

                trip.TripRequest.Add(tripRequest);

                await this.tripRequestsRepository.AddAsync(tripRequest);
                await this.tripRequestsRepository.SaveChangesAsync();

                var notificationId = await this.notificationService.AddTripRequestNotification(sender.UserName, owner.UserName, $"{sender.UserName} sent a trip request from your trip!", trip.Id);
                var notification = await this.notificationService.GetNotificationByIdAsync(notificationId);
                await this.hubContext.Clients.User(ownerId).SendAsync("VisualizeNotification", notification);
                return isRequestSent;
            }
        }

        public async Task<bool> IsRequesAlreadySend(string senderId, string tripId)
        {
            return await this.tripRequestsRepository.All().AnyAsync(x => x.SenderId == senderId && x.TripId == tripId);
        }
    }
}
