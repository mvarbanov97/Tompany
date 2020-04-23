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
using Tompany.Web.ViewModels.Trips;
using Tompany.Data.Models.Enums;

namespace Tompany.Services.Data
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Trip> tripsRepository;
        private readonly IRepository<TripRequest> tripRequestRepository;

        public UsersService(
            IRepository<ApplicationUser> usersRepository,
            IRepository<Trip> tripsRepository,
            IRepository<TripRequest> tripRequestRepository)
        {
            this.usersRepository = usersRepository;
            this.tripsRepository = tripsRepository;
            this.tripRequestRepository = tripRequestRepository;
        }

        public ApplicationUser GetUserById(string id)
        {
            var user = this.usersRepository.All().FirstOrDefault(x => x.Id == id);

            return user;
        }

        public async Task AcceptTripRequest(string senderId, string tripId, string userId)
        {
            var user = this.GetUserById(userId);
            var sender = this.GetUserById(senderId);

            var trip = this.tripsRepository
                .All()
                .Include(x => x.TripRequest)
                .Where(x => x.TripRequest.Any(x => x.TripId == tripId))
                .FirstOrDefault();

            var tripRequest = trip.TripRequest.Where(x => x.SenderId == senderId && x.TripId == tripId).FirstOrDefault();

            tripRequest.RequestStatus = RequestStatus.Accepted;

            this.tripRequestRepository.Update(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();
        }
    }
}
