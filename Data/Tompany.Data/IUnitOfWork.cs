using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;

namespace Tompany.Data
{
    public interface IUnitOfWork
    {
        IDeletableEntityRepository<Car> Cars { get; }

        IDeletableEntityRepository<ApplicationUser> Users { get; }

        IDeletableEntityRepository<Group> Groups { get; }

        IDeletableEntityRepository<ChatMessage> ChatMessages { get; }

        IRepository<Destination> Destinations { get; }

        IRepository<Trip> Trips { get; }

        IRepository<TripRequest> TripRequests { get; }

        IRepository<UserTrip> UserTrips { get; }

        IRepository<UserRating> UserRatings { get; }
        Task CompleteAsync();
    }
}
