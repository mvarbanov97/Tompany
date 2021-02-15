namespace Tompany.Data
{
    using System.Threading.Tasks;

    using Tompany.Data.Common.Repositories;
    using Tompany.Data.Models;
    using Tompany.Data.Repositories;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            this.Cars = new EfDeletableEntityRepository<Car>(context);
            this.Users = new EfDeletableEntityRepository<ApplicationUser>(context);
            this.Groups = new EfDeletableEntityRepository<Group>(context);
            this.ChatMessages = new EfDeletableEntityRepository<ChatMessage>(context);
            this.Destinations = new EfRepository<Destination>(context);
            this.Trips = new EfRepository<Trip>(context);
            this.TripRequests = new EfRepository<TripRequest>(context);
            this.UserTrips = new EfRepository<UserTrip>(context);
            this.UserRatings = new EfRepository<UserRating>(context);
        }

        public IDeletableEntityRepository<Car> Cars { get; private set; }

        public IDeletableEntityRepository<ApplicationUser> Users { get; private set; }

        public IDeletableEntityRepository<Group> Groups { get; private set; }

        public IDeletableEntityRepository<ChatMessage> ChatMessages { get; private set; }

        public IRepository<Destination> Destinations { get; private set; }

        public IRepository<Trip> Trips { get; private set; }

        public IRepository<TripRequest> TripRequests { get; private set; }

        public IRepository<UserTrip> UserTrips { get; private set; }

        public IRepository<UserRating> UserRatings { get; private set; }

        public async Task CompleteAsync()
        {
            await this.context.SaveChangesAsync();
        }
    }
}
