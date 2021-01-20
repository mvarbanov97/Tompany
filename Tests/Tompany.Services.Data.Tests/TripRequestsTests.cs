using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Data.Repositories;
using Tompany.Web.Infrastructure;
using Tompany.Web.Infrastructure.Contracts;
using Tompany.Web.Infrastructure.Hubs;
using Xunit;

namespace Tompany.Services.Data.Tests
{
    public class TripRequestsTests
    {
        [Fact]
        public async Task GetByIdShouldReturnEntitySuccessfully()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TripRequestByIdDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripReqeustRepository = new EfDeletableEntityRepository<TripRequest>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var mockHub = new Mock<IHubContext<NotificationHub>>().Object;
            var mockService = new Mock<INotificationService>().Object;

            var service = new TripRequestsService(tripReqeustRepository, usersRepository, mockHub, mockService);
            var tripRequestId = Guid.NewGuid().ToString();

            await dbContext.AddAsync(new TripRequest
            {
                Id = tripRequestId,
            });
            await dbContext.SaveChangesAsync();

            var fetchedTripRequest = service.GetById(tripRequestId);
            var expectedTripRequest = await dbContext.TripRequests.FirstOrDefaultAsync(x => x.Id == tripRequestId);

            Assert.Equal(expectedTripRequest, fetchedTripRequest);
        }

        [Fact]
        public async Task GetAllRequestByTripIdReturnCorrectEntities()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AllTripRequestByIdDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripReqeustRepository = new EfDeletableEntityRepository<TripRequest>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var mockHub = new Mock<IHubContext<NotificationHub>>().Object;
            var mockService = new Mock<INotificationService>().Object;

            var service = new TripRequestsService(tripReqeustRepository, usersRepository, mockHub, mockService);
            var tripRequestId = Guid.NewGuid().ToString();

            var tripRequestOne = new TripRequest { Id = "testOne", TripId = "same" };
            var tripRequestTwo = new TripRequest { Id = "testTwo", TripId = "same" };
            await dbContext.TripRequests.AddRangeAsync(tripRequestOne, tripRequestTwo);
            await dbContext.SaveChangesAsync();

            var actualTrips = service.GetAllTripRequestsByTripId("same");

            Assert.Equal(2, actualTrips.Count());
        }

        [Fact]
        public async Task GetAllPendingTripReuqestByTripIdReturnCorrectEntities()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AllPendingTripRequestByIdDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripReqeustRepository = new EfDeletableEntityRepository<TripRequest>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var mockHub = new Mock<IHubContext<NotificationHub>>().Object;
            var mockService = new Mock<INotificationService>().Object;

            var service = new TripRequestsService(tripReqeustRepository, usersRepository, mockHub, mockService);
            var tripRequestId = Guid.NewGuid().ToString();

            var tripRequestOne = new TripRequest { Id = "testOne", TripId = "same", RequestStatus = RequestStatus.Pending };
            var tripRequestTwo = new TripRequest { Id = "testTwo", TripId = "same", RequestStatus = RequestStatus.Accepted };
            var tripRequestThree = new TripRequest { Id = "testThree", TripId = "same", RequestStatus = RequestStatus.Declined };
            await dbContext.TripRequests.AddRangeAsync(tripRequestOne, tripRequestTwo);
            await dbContext.SaveChangesAsync();

            var actualTripReqeusts = service.GetPendingRequestsByTripId("same");

            Assert.Single(actualTripReqeusts);
        }

        [Fact]
        public async Task SendTripRequestShouldAddEntityToDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SendTripRequestDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripReqeustRepository = new EfDeletableEntityRepository<TripRequest>(dbContext);
            var tripRepository = new EfDeletableEntityRepository<Trip>(dbContext);
            var destinationRepository = new EfRepository<Destination>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var userNotificationsRepository = new EfRepository<UserNotification>(dbContext);
            var notificationService = new NotificationService(usersRepository, userNotificationsRepository);

            var mockClientProxy = new Mock<IClientProxy>();
            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            mockClients.Setup(c => c.User("awe22")).Returns(mockClientProxy.Object);

            var hub = new Mock<IHubContext<NotificationHub>>();
            hub.Setup(x => x.Clients).Returns(() => mockClients.Object);
            var tripService = new TripsService(tripRepository, destinationRepository, usersRepository);

            var service = new TripRequestsService(tripReqeustRepository, usersRepository, hub.Object, notificationService);
            await dbContext.Trips.AddAsync(new Trip
            {
                Id = "123",
                FromDestinationName = "sofia",
                ToDestinationName = "Silistra",
            });

            var sender = new ApplicationUser
            {
                Id = "awe12",
                UserName = "robota",
            };
            var owner = new ApplicationUser
            {
                Id = "awe22",
                UserName = "robota2",
            };

            await dbContext.Users.AddRangeAsync(sender, owner);
            await dbContext.SaveChangesAsync();

            using (dbContext)
            {
                var trip = await dbContext.Trips.FirstOrDefaultAsync(x => x.Id == "123");
                var actual = await service.SendTripRequest(sender.UserName, trip, owner.Id);

                Assert.True(actual);
            }
        }

        [Fact]
        public async Task IsRequestAlreadySendReturningCorrectValue()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TripRequestAlreadySendDb").Options;
            var dbContext = new ApplicationDbContext(options);

            var tripReqeustRepository = new EfDeletableEntityRepository<TripRequest>(dbContext);
            var usersRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var mockHub = new Mock<IHubContext<NotificationHub>>().Object;
            var mockService = new Mock<INotificationService>().Object;

            var service = new TripRequestsService(tripReqeustRepository, usersRepository, mockHub, mockService);
            var senderId = Guid.NewGuid().ToString();

            var tripRequestOne = new TripRequest { Id = "testOne", TripId = "same", RequestStatus = RequestStatus.Pending, SenderId = senderId };
            await dbContext.TripRequests.AddAsync(tripRequestOne);
            await dbContext.SaveChangesAsync();

            var isSend = await service.IsRequesAlreadySend(senderId, "same");

            Assert.True(isSend);
        }
    }
}