using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Web.Infrastructure;

namespace Tompany.Controllers.Tests
{
    public static class ChatsControllerExtension
    {
        public static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var currentUser = new ApplicationUser { Id = "1", UserName = "testUsername" };

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                    new Mock<IUserStore<ApplicationUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<ApplicationUser>>().Object,
                    new IUserValidator<ApplicationUser>[0],
                    new IPasswordValidator<ApplicationUser>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                 .ReturnsAsync(currentUser);

            return mockUserManager;
        }

        public static Mock<IChatService> MockChatService()
        {
            var mockChatService = new Mock<IChatService>();

            mockChatService.Setup(x => x.ExtractAllMessages(It.IsAny<string>()))
                .ReturnsAsync(new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Content = "testContent"
                    },
                    new ChatMessage
                    {
                        Content = "testContent2"
                    },
                });

            return mockChatService;
        }

        public static Mock<IUnitOfWork> MockUnitOfWork()
        {
            var mockUserRepository = new Mock<IDeletableEntityRepository<ApplicationUser>>();

            var testTrip = new Trip() { Id = "1" };
            var listTrip = new List<Trip>();
            listTrip.Add(testTrip);

            var mockTripRepository = new Mock<IRepository<Trip>>();
            mockTripRepository.Setup(x => x.All()).Returns(listTrip.AsQueryable());

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.Trips.All()).Returns(It.IsAny<IQueryable<Trip>>());
            mockUnitOfWork.Setup(u => u.Users).Returns(mockUserRepository.Object).Verifiable();
            mockUnitOfWork.Setup(u => u.Trips).Returns(mockTripRepository.Object).Verifiable(); // first trip with given ID

            var testReturn = mockUnitOfWork.Object.Trips.All();

            return mockUnitOfWork;
        }

    }
}
