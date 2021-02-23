using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services;
using Tompany.Services.Data.Contracts;
using Tompany.Web.Controllers;
using Tompany.Web.ViewModels.Cars.InputModels;
using Tompany.Web.ViewModels.Cars.ViewModels;

namespace Tompany.Controllers.Tests
{
    public static class CarsControllerExtension
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

        public static Mock<ICarsService> MockCarService()
        {
            var mockCarService = new Mock<ICarsService>();
            mockCarService.Setup(x => x.GetAllUserCarsByUserId<CarViewModel>("1"))
                .Returns(new List<CarViewModel>
                {
                    new CarViewModel
                    {
                        Id = 1,
                        Model = "Test1",
                    },
                    new CarViewModel
                    {
                        Id = 1,
                        Model = "Test2",
                    }
                });

            mockCarService.Setup(x => x.CreateAsync("1", new CarCreateInputModel { Brand = "TestBrand", Model = "TestModel", Seats = 3 }));
            mockCarService.Setup(x => x.EditAsync(new CarEditIputModel { Brand = "TestBrand", Model = "TestModel", Seats = 3 }, "1"));
            mockCarService.Setup(x => x.IsCarExists(It.IsAny<int>(), It.IsAny<string>()));
            mockCarService.Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<string>()));
            mockCarService.Setup(x => x.ExtractCar(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new CarEditIputModel
                {
                    Id = 1,
                    UserId = "1",
                });

            return mockCarService;
        }

        public static Mock<ICloudinaryService> MockCloudinaryService()
        {
            var mockCloudinaryService = new Mock<ICloudinaryService>();

            mockCloudinaryService.Setup(x => x.UploadImageAsync(null, null)).ReturnsAsync("testPictureName");

            return mockCloudinaryService;
        }
    }
}
