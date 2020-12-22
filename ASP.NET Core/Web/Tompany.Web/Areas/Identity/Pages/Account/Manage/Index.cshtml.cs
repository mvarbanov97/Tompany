using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tompany.Common;
using Tompany.Data.Models;
using Tompany.Services;
using Tompany.Web.Areas.Identity.Pages.Account.InputModels;

namespace Tompany.Web.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ICloudinaryService cloudinaryService;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICloudinaryService cloudinaryService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.cloudinaryService = cloudinaryService;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public ManageAccountInputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await this.userManager.GetUserNameAsync(user);
            var phoneNumber = await this.userManager.GetPhoneNumberAsync(user);

            this.Username = userName;

            this.Input = new ManageAccountInputModel
            {
                PhoneNumber = phoneNumber,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                AboutMe = user.AboutMe,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GitHubUrl = user.GitHubUrl,
                StackoverflowUrl = user.StackoverflowUrl,
                FacebookUrl = user.FacebookUrl,
                InstagramUrl = user.InstagramUrl,
                TwitterUrl = user.TwitterUrl,
                LinkedinUrl = user.LinkedinUrl,
                RegisteredOn = user.RegisteredOn,
                Email = user.Email,

                // TODO Image URL
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            await this.LoadAsync(user);
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            bool isUpdatePersonalData = false;
            bool isUpdateProfileImage = false;
            bool isUpdateCoverImage = false;

            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            if (!this.ModelState.IsValid)
            {
                await this.LoadAsync(user);
                return this.Page();
            }

            var phoneNumber = await this.userManager.GetPhoneNumberAsync(user);
            if (this.Input.PhoneNumber != phoneNumber && this.Input.PhoneNumber != null)
            {
                var setPhoneResult = await this.userManager.SetPhoneNumberAsync(user, this.Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await this.userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }

                isUpdatePersonalData = true;
            }

            if (this.Input.BirthDate != user.BirthDate)
            {
                user.BirthDate = this.Input.BirthDate;
                isUpdatePersonalData = true;
            }

            if (this.Input.Gender != user.Gender)
            {
                user.Gender = this.Input.Gender;
                isUpdatePersonalData = true;
            }

            if (this.Input.AboutMe != user.AboutMe)
            {
                user.AboutMe = this.Input.AboutMe;
                isUpdatePersonalData = true;
            }

            if (this.Input.FirstName != user.FirstName)
            {
                user.FirstName = this.Input.FirstName;
                isUpdatePersonalData = true;
            }

            if (this.Input.LastName != user.LastName)
            {
                user.LastName = this.Input.LastName;
                isUpdatePersonalData = true;
            }

            if (this.Input.GitHubUrl != user.GitHubUrl)
            {
                user.GitHubUrl = this.Input.GitHubUrl;
                isUpdatePersonalData = true;
            }

            if (this.Input.StackoverflowUrl != user.StackoverflowUrl)
            {
                user.StackoverflowUrl = this.Input.StackoverflowUrl;
                isUpdatePersonalData = true;
            }

            if (this.Input.FacebookUrl != user.FacebookUrl)
            {
                user.FacebookUrl = this.Input.FacebookUrl;
                isUpdatePersonalData = true;
            }

            if (this.Input.LinkedinUrl != user.LinkedinUrl)
            {
                user.LinkedinUrl = this.Input.LinkedinUrl;
                isUpdatePersonalData = true;
            }

            if (this.Input.TwitterUrl != user.TwitterUrl)
            {
                user.TwitterUrl = this.Input.TwitterUrl;
                isUpdatePersonalData = true;
            }

            if (this.Input.InstagramUrl != user.InstagramUrl)
            {
                user.InstagramUrl = this.Input.InstagramUrl;
                isUpdatePersonalData = true;
            }

            if (this.Input.ProfilePicture != null)
            {
                var profileImageUrl = await this.cloudinaryService.UploadImageAsync(
                this.Input.ProfilePicture,
                string.Format(GlobalConstants.CloudinaryProfilePictureName, user.UserName));

                if (profileImageUrl != null)
                {
                    isUpdateProfileImage = true;
                    if (profileImageUrl != user.ImageUrl)
                    {
                        user.ImageUrl = profileImageUrl;
                    }
                }
            }
            

            //var coverImageUrl = await ApplicationCloudinary.UploadImage(
            //    this.cloudinary,
            //    this.Input.CoverImage,
            //    string.Format(GlobalConstants.CloudinaryUserCoverImageName, user.UserName));

            //if (coverImageUrl != null)
            //{
            //    isUpdateCoverImage = true;
            //    if (coverImageUrl != user.ImageUrl)
            //    {
            //        user.CoverImageUrl = coverImageUrl;
            //    }
            //}

            await this.userManager.UpdateAsync(user);
            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "Your profile has been updated";
            return this.RedirectToPage();
        }
    }
}
