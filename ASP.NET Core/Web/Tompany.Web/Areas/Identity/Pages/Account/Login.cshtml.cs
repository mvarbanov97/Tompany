using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tompany.Data.Models;

namespace Tompany.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LoginModel> logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Потребителско име или парола")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запомни ме")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
            }

            if (this.User.Identity.IsAuthenticated)
            {
                this.Response.Redirect("/Home/Error");
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.Forbid();
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            ApplicationUser user = this.Input.Email
                .Contains('@')
                ? await this.userManager.FindByEmailAsync(this.Input.Email)
                : await this.userManager.FindByNameAsync(this.Input.Email);

            if (this.ModelState.IsValid)
            {
                if (user != null)
                {
                    if (user.IsDeleted)
                    {
                        return this.Page();
                    }

                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await this.signInManager.PasswordSignInAsync(user, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        this.logger.LogInformation("User logged in.");
                        return this.LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = this.Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        this.logger.LogWarning("User account locked out.");
                        return this.RedirectToPage("./Lockout");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return this.Page();
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Невалидно потребителско име или парола!");
                    return this.Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
