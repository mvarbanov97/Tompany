namespace Tompany.Areas.Identity.Pages.Account
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Options;
    using Tompany.Data;
    using Tompany.Data.Models;
    using Tompany.Services.Messaging.SecurityModels;
    using Twilio.Rest.Verify.V2.Service;

    [Authorize]
    public class ConfirmPhoneModel : PageModel
    {
        private readonly TwilioVerifySettings settings;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext db;

        public ConfirmPhoneModel(
            UserManager<ApplicationUser> userManager,
            IOptions<TwilioVerifySettings> settings,
            ApplicationDbContext db)
        {
            this.userManager = userManager;
            this.db = db;
            this.settings = settings.Value;
        }

        public string PhoneNumber { get; set; }

        public string CountryCode { get; set; }

        [BindProperty]
        [Required]
        [Display(Name = "Code")]
        public string VerificationCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await this.LoadPhoneNumber();
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await this.LoadPhoneNumber();
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            try
            {
                var verification = await VerificationCheckResource.CreateAsync(
                    to: $"{this.CountryCode}{this.PhoneNumber}",
                    code: this.VerificationCode,
                    pathServiceSid: this.settings.VerificationServiceSID);

                if (verification.Status == "approved")
                {
                    var identityUser = await this.userManager.GetUserAsync(this.User);
                    identityUser.PhoneNumberConfirmed = true;
                    var updateResult = await this.userManager.UpdateAsync(identityUser);

                    if (updateResult.Succeeded)
                    {
                        var user = this.userManager.GetUserAsync(this.HttpContext.User);

                        return this.Redirect($"/Profile/{user.Result.UserName}");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, "There was an error confirming the verification code, please try again");
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, $"There was an error confirming the verification code: {verification.Status}");
                }
            }
            catch (Exception)
            {
                this.ModelState.AddModelError(
                    string.Empty,
                    "There was an error confirming the code, please check the verification code is correct and try again");
            }

            return this.Page();
        }

        private async Task LoadPhoneNumber()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                throw new Exception($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            this.PhoneNumber = user.PhoneNumber;
            this.CountryCode = this.db.CountryCodes.FirstOrDefault(x => x.Id == user.CountryCodeId).Code;
        }
    }
}