using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;

namespace Tompany.Services.Data.Contracts
{
    public interface ITripRequestsService
    {
        TripRequest GetById(string id);

        IEnumerable<TripRequest> GetAllTripRequestsByTripId(string tripId);

        IEnumerable<TripRequest> GetPendingRequestsByTripId(string tripId);

        Task<bool> SendTripRequest(string senderId, Trip trip, string ownerId);

        Task<bool> IsRequesAlreadySend(string senderId, string tripId);
    }
}
