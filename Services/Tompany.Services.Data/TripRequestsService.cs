namespace Tompany.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Tompany.Data;
    using Tompany.Data.Models;
    using Tompany.Data.Models.Enums;
    using Tompany.Services.Data.Contracts;
    using Tompany.Web.Infrastructure.Contracts;
    using Tompany.Web.Infrastructure.Hubs;

    public class TripRequestsService : ITripRequestsService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IHubContext<NotificationHub> hubContext;
        private readonly INotificationService notificationService;

        public TripRequestsService(
            IHubContext<NotificationHub> hubContext,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            this.hubContext = hubContext;
            this.unitOfWork = unitOfWork;
            this.notificationService = notificationService;
        }

        public TripRequest GetById(string id)
        {
            return this.unitOfWork.TripRequests
                                  .All()
                                  .Where(x => x.Id == id)
                                  .FirstOrDefault();
        }

        public IEnumerable<TripRequest> GetAllTripRequestsByTripId(string tripId)
        {
            return this.unitOfWork.TripRequests.All()
                                               .Include(x => x.Sender)
                                               .Where(x => x.TripId == tripId);
        }

        public IEnumerable<TripRequest> GetPendingRequestsByTripId(string tripId)
        {
            return this.unitOfWork.TripRequests.All()
                                               .Include(x => x.Sender)
                                               .Include(x => x.Trip)
                                               .Where(x => x.TripId == tripId && x.RequestStatus == RequestStatus.Pending);
        }

        public async Task<bool> SendTripRequest(string userName, Trip trip, string ownerId)
        {
            var isRequestSent = false;

            var owner = await this.unitOfWork.Users
                .All()
                .Where(x => x.Id == ownerId)
                .Include(x => x.UserTrips)
                .Include(x => x.TripRequests)
                .FirstOrDefaultAsync();

            var sender = await this.unitOfWork.Users
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

                await this.unitOfWork.TripRequests.AddAsync(tripRequest);
                await this.unitOfWork.CompleteAsync();

                var notificationId = await this.notificationService.AddTripRequestNotification(sender.UserName, owner.UserName, $"{sender.UserName} sent a trip request from your trip!", trip.Id);
                var notification = await this.notificationService.GetNotificationByIdAsync(notificationId);
                await this.hubContext.Clients.User(ownerId).SendAsync("VisualizeNotification", notification);
                return isRequestSent;
            }
        }

        public async Task<bool> IsRequesAlreadySend(string senderId, string tripId)
        {
            return await this.unitOfWork.TripRequests
                                        .All()
                                        .AnyAsync(x => x.SenderId == senderId && x.TripId == tripId);
        }
    }
}
