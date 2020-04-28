using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Common;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Messaging;
using Tompany.Web.ViewModels.Contacts;

namespace Tompany.Web.Controllers
{
    public class ContactsController : BaseController
    {
        public readonly IRepository<ContactFormEntry> contactsRepository;

        public readonly IEmailSender emailSender;

        public ContactsController(
            IRepository<ContactFormEntry> contactsRepository,
            IEmailSender emailSender)
        {
            this.contactsRepository = contactsRepository;
            this.emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactFormViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
            var contactFormEntry = new ContactFormEntry
            {
                Name = model.Name,
                Email = model.Email,
                Title = model.Title,
                Content = model.Content,
            };
            await this.contactsRepository.AddAsync(contactFormEntry);
            await this.contactsRepository.SaveChangesAsync();

            await this.emailSender.SendEmailAsync(
                model.Email,
                model.Name,
                GlobalConstants.SystemEmail,
                model.Title,
                model.Content);

            return this.RedirectToAction("ContactSuccessfullySent");
        }

        public IActionResult ContactSuccessfullySent()
        {
            return this.View();
        }
    }
}
