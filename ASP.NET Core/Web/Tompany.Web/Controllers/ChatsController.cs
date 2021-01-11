﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tompany.Data;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Web.Infrastructure;
using Tompany.Web.Infrastructure.Hubs;
using Tompany.Web.ViewModels.Chat;

namespace Tompany.Web.Controllers
{
    [Authorize]
    public class ChatsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IChatService chatService;

        public ChatsController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IChatService chatService)
        {
            this.db = db;
            this.userManager = userManager;
            this.chatService = chatService;
        }

        [Route("Chat/With/{username?}/Group/{group?}")]
        public async Task<IActionResult> Index(string username, string group)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            var model = new ChatViewModel
            {
                FromUser = await this.userManager.GetUserAsync(this.HttpContext.User),
                ToUser = this.db.Users.FirstOrDefault(x => x.UserName == username),
                ChatMessages = await this.chatService.ExtractAllMessages(group),
                GroupName = group,
            };

            return this.View(model);
        }

        [HttpGet]
        [Route("Chat/GroupChat/{group}")]
        public async Task<IActionResult> GroupChat(string group, string tripId)
        {
            var currentUser = await this.userManager.GetUserAsync(this.User);

            var trip = await this.db.Trips.Where(x => x.Id == tripId).Include(x => x.User).Include(x => x.Passengers).FirstOrDefaultAsync();

            var model = new GroupChatViewModel
            {
                Users = trip.Passengers,
                ChatMessages = await this.chatService.ExtractAllMessages(group),
                GroupName = group,
            };

            return this.View(model);
        }
    }
}
