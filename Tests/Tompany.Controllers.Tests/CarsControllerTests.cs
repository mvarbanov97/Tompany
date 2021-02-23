using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Services;
using Tompany.Services.Data.Contracts;
using Tompany.Web.Controllers;
using Tompany.Web.ViewModels.Cars.InputModels;
using Tompany.Web.ViewModels.Cars.ViewModels;
using Xunit;

namespace Tompany.Controllers.Tests
{
    public class CarsControllerTests
    {
        private CarsController sut;
        private Mock<UserManager<ApplicationUser>> userManager;
        private Mock<ICarsService> mockCarService;
        private Mock<ICloudinaryService> mockCloudinaryService;

        public CarsControllerTests()
        {
            mockCarService = CarsControllerExtension.MockCarService();
            userManager = CarsControllerExtension.MockUserManager();
            mockCloudinaryService = CarsControllerExtension.MockCloudinaryService();

            sut = new CarsController(
                mockCarService.Object,
                userManager.Object,
                mockCloudinaryService.Object);
        }

        [Fact]
        [Trait("Get Requests", "Index")]
        public async Task IndexShouldReturnViewResultWithAllUserCars()
        {
            //Act
            var result = await this.sut.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarListViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Cars.Count());
        }

        [Fact]
        [Trait("Get Requests", "Create")]
        public async Task GetCreateShouldReturnCorrectViewResult()
        {
            //Act
            var result = this.sut.Create();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        [Trait("Post Requests", "Create with valid model")]
        public async Task PostCreateShouldRedirectToActionSuccessfullyWhenModelIsValid()
        {
            //Act
            var result = await this.sut.Create(new CarCreateInputModel());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Cars", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        [Trait("Post Requests", "Create with invalid model")]
        public async Task PostCreateShouldReturnViewResultSuccessfullyWhenModelIsInvalid()
        {
            //Arrange
            this.sut.ModelState.AddModelError("Brand", "Required");

            //Act
            var result = this.sut.Create(new CarCreateInputModel());
            //Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);
        }

        [Fact]
        [Trait("Get Requests", "Edit")]
        public async Task GetEditShouldReturnViewResultWithCorrectModelSuccessfully()
        {
            //Arrange
            mockCarService.Setup(x => x.IsCarExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            //Act
            var result = await this.sut.Edit(new int());

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CarEditIputModel>(
                viewResult.ViewData.Model);
        }

        [Fact]
        [Trait("Get Requests", "Edit when user is unauthorized")]
        public async Task GetEditShouldReturnUnauthorizedWhenCarIsNotFound()
        {
            this.mockCarService.Setup(x => x.IsCarExists(It.IsAny<int>(), It.IsAny<string>())).Returns(false);
            //Act
            var result = await this.sut.Edit(new int());

            //Assert
            var viewResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        [Trait("Post Requests", "Edit with valid model")]
        public async Task PostEditShouldRedirectToActionSuccessfullyIfModelIsValid()
        {
            //Act
            var result = await this.sut.Edit(new CarEditIputModel()) as RedirectToActionResult;

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", result.ControllerName);
            Assert.Equal("Profile", result.ActionName);
            Assert.Equal("UserAllRegisteredVehicles", result.RouteValues.GetValueOrDefault("tab"));
        }

        [Fact]
        [Trait("Post Requests", "Edit with invalid model")]
        public async Task PostEditShouldReturnViewResultIfModelIsInvalid()
        {
            //Arrange
            this.sut.ModelState.AddModelError("Brand", "Required");

            //Act
            var result = await this.sut.Edit(new CarEditIputModel());

            //Assert
            var redirectToActionResult = Assert.IsType<ViewResult>(result);
            this.mockCarService.Verify(x => x.EditAsync(It.IsAny<CarEditIputModel>(), "1"), Times.Never);
        }

        [Fact]
        [Trait("Post Requests", "Delete existing car")]
        public async Task DeleteShouldRemoveCarIfItExists()
        {
            //Arrange
            mockCarService.Setup(x => x.IsCarExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            //Act
            var result = await this.sut.Delete(It.IsAny<int>());

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Users", redirectToActionResult.ControllerName);
            Assert.Equal("Profile", redirectToActionResult.ActionName);

            redirectToActionResult.RouteValues.TryGetValue("tab", out object tab);
            Assert.Equal("UserAllRegisteredVehicles", tab);
        }

        [Fact]
        [Trait("Post Requests", "Delete invalid car")]
        public async Task DeleteShouldReturnUnauthorizedIfUserIsTryingToRemoveOtherUserCarCar()
        {
            //Act
            var result = await this.sut.Delete(It.IsAny<int>());
            var contentResult = result as StatusCodeResult;

            //Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, contentResult.StatusCode);
        }
    }
}
