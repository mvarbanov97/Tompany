using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Common;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Messaging;
using Tompany.Web.ViewModels.Contacts.InputModels;

namespace Tompany.Web.Controllers
{
    public class ContactsController : BaseController
    {
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public ContactsController(
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager = null)
        {
            this.emailSender = emailSender;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.Identity.IsAuthenticated == true)
            {
                var user = await this.userManager.GetUserAsync(this.User);
                var email = await this.userManager.GetEmailAsync(user);

                var viewModel = new ContactInputModel { Name = user.UserName, Email = email };

                return this.View(viewModel);
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(ContactInputModel inputModel)
        {
            await this.emailSender.SendEmailAsync(inputModel.Email, inputModel.Name, GlobalConstants.SystemEmail, "Contact request", inputModel.Content);

            return this.RedirectToAction("Index");
        }
    }
}
