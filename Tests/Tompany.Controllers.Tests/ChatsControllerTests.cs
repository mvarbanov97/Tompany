using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.Controllers;
using Tompany.Web.Infrastructure;
using Tompany.Web.ViewModels.Chat;
using Xunit;

namespace Tompany.Controllers.Tests
{
    public class ChatsControllerTests
    {
        private ChatsController sut;
        private Mock<UserManager<ApplicationUser>> userManager;
        private Mock<IChatService> mockChatService;
        private Mock<IUnitOfWork> mockUnitOfWork;

        public ChatsControllerTests()
        {
            this.userManager = ChatsControllerExtension.MockUserManager();
            this.mockChatService = ChatsControllerExtension.MockChatService();
            this.mockUnitOfWork = ChatsControllerExtension.MockUnitOfWork();

            this.sut = new ChatsController(userManager.Object, mockChatService.Object, mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetIndexShouldReturnViewResultSuccessfully()
        {
            //Act
            var result = await this.sut.Index("testUsername", "testGroup");

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ChatViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task GetGroupChatShouldReturnViewResultSuccessfully()
        {
            //Arrange


            //Act
            var result = await this.sut.GroupChat("testGroup", "1");

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<GroupChatViewModel>(viewResult.Model);
        }



    }
}
