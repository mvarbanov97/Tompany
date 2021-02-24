using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tompany.Common;
using Tompany.Data;
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
        private readonly IUnitOfWork unitOfWork;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICloudinaryService cloudinaryService, 
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.cloudinaryService = cloudinaryService;
            this.unitOfWork = unitOfWork;
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
            var zipCode = this.unitOfWork.ZipCodes.All().FirstOrDefault(x => x.Id == user.ZipCodeId);
            var country = this.unitOfWork.Countries.All().FirstOrDefault(x => x.Id == user.CountryId);
            var state = this.unitOfWork.States.All().FirstOrDefault(x => x.Id == user.StateId);
            var city = this.unitOfWork.Cities.All().FirstOrDefault(x => x.Id == user.CityId);
            var countryCode = this.unitOfWork.CountryCodes.All().FirstOrDefault(x => x.Id == user.CountryCodeId);

            var allCountryCodesNames = this.unitOfWork.CountryCodes.All().Select(x => x.Code).OrderBy(x => x).ToList();
            var allCities = this.unitOfWork.Cities.All().Select(x => x.Name).OrderBy(x => x).ToList();
            var allStates = this.unitOfWork.States.All().Select(x => x.Name).OrderBy(x => x).ToList();
            var allCountries = this.unitOfWork.Countries.All().Select(x => x.Name).OrderBy(x => x).ToList();

            this.Username = userName;

            this.Input = new ManageAccountInputModel
            {
                PhoneNumber = phoneNumber,
                State = state?.Name,
                Country = country?.Name,
                City = city?.Name,
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
                CountryCode = countryCode?.Code,
                Email = user.Email,
                ZipCode = zipCode.Code.ToString(),
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

            CountryCode targetCountryCode = await this.ValidateCountryCode(
                    this.Input.CountryCode);

            Country targetCountry = await this.ValidateCountry(
                this.Input.Country,
                targetCountryCode);

            State targetState = await this.ValidateState(
                this.Input.State,
                targetCountry);

            City targetCity = await this.ValidateCity(
                this.Input.City,
                targetState,
                targetCountry);

            ZipCode targetZipCode = await this.ValidateZipCode(
                this.Input.ZipCode,
                targetCity);

            if (user.CountryCodeId != targetCountryCode?.Id)
            {
                user.CountryCodeId = targetCountryCode?.Id;
                isUpdatePersonalData = true;
            }

            if (user.CountryId != targetCountry?.Id)
            {
                user.CountryId = targetCountry?.Id;
                isUpdatePersonalData = true;
            }

            if (user.StateId != targetState?.Id)
            {
                user.StateId = targetState?.Id;
                isUpdatePersonalData = true;
            }

            if (user.CityId != targetCity?.Id)
            {
                user.CityId = targetCity?.Id;
                isUpdatePersonalData = true;
            }

            if (user.ZipCodeId != targetZipCode?.Id)
            {
                user.ZipCodeId = targetZipCode?.Id;
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

            await this.userManager.UpdateAsync(user);
            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "Your profile has been updated";
            return this.RedirectToPage();
        }

        private async Task<City> ValidateCity(string city, State targetState, Country targetCountry)
        {
            if (city != null)
            {
                var targetCity = await this.unitOfWork.Cities.All()
                    .FirstOrDefaultAsync(x => x.Name.ToUpper() == city.ToUpper());

                if (targetCity == null)
                {
                    targetCity = new City
                    {
                        Name = city,
                        CountryId = targetCountry?.Id,
                        StateId = targetState?.Id,
                    };

                    await this.unitOfWork.Cities.AddAsync(targetCity);
                    await this.unitOfWork.CompleteAsync();
                }
                else
                {
                    if (targetCountry != null && targetCountry.Id != targetCity.CountryId)
                    {
                        targetCity.CountryId = targetCountry.Id;

                        this.unitOfWork.Cities.Update(targetCity);
                        await this.unitOfWork.CompleteAsync();
                    }

                    if (targetState != null && targetState.Id != targetCity.StateId)
                    {
                        targetCity.StateId = targetState.Id;

                        this.unitOfWork.Cities.Update(targetCity);
                        await this.unitOfWork.CompleteAsync();
                    }
                }

                return targetCity;
            }

            return null;
        }

        private async Task<State> ValidateState(string state, Country targetCountry)
        {
            if (state != null)
            {
                var targetSate = await this.unitOfWork.States.All()
                    .FirstOrDefaultAsync(x => x.Name.ToUpper() == state.ToUpper());

                if (targetSate == null)
                {
                    targetSate = new State
                    {
                        Name = state,
                        CountryId = targetCountry?.Id,
                    };

                    await this.unitOfWork.States.AddAsync(targetSate);
                    await this.unitOfWork.CompleteAsync();
                }
                else
                {
                    if (targetCountry != null && targetCountry.Id != targetSate.CountryId)
                    {
                        targetSate.CountryId = targetCountry.Id;

                        this.unitOfWork.States.Update(targetSate);
                        await this.unitOfWork.CompleteAsync();
                    }
                }

                return targetSate;
            }

            return null;
        }

        private async Task<Country> ValidateCountry(string country, CountryCode targetCountryCode)
        {
            if (country != null)
            {
                var targetCountry = await this.unitOfWork.Countries.All()
                        .FirstOrDefaultAsync(x => x.Name.ToUpper() == country.ToUpper());

                if (targetCountry == null)
                {
                    targetCountry = new Country
                    {
                        Name = country,
                        CountryCodeId = targetCountryCode?.Id,
                    };

                    await this.unitOfWork.Countries.AddAsync(targetCountry);
                    await this.unitOfWork.CompleteAsync();
                }
                else
                {
                    if (targetCountryCode != null && targetCountryCode.Id != targetCountry.CountryCodeId)
                    {
                        targetCountry.CountryCodeId = targetCountryCode.Id;

                        this.unitOfWork.Countries.Update(targetCountry);
                        await this.unitOfWork.CompleteAsync();
                    }
                }

                return targetCountry;
            }

            return null;
        }

        private async Task<CountryCode> ValidateCountryCode(string countryCode)
        {
            if (countryCode != null)
            {
                var targetCountryCode = await this.unitOfWork.CountryCodes.All()
                        .FirstOrDefaultAsync(x => x.Code.ToUpper() == countryCode.ToUpper());

                if (targetCountryCode == null)
                {
                    targetCountryCode = new CountryCode
                    {
                        Code = countryCode,
                    };

                    await this.unitOfWork.CountryCodes.AddAsync(targetCountryCode);
                    await this.unitOfWork.CompleteAsync();
                }

                return targetCountryCode;
            }

            return null;
        }

        private async Task<ZipCode> ValidateZipCode(string zipCode, City targetCity)
        {
            if (zipCode != null)
            {
                var targetZipCode = await this.unitOfWork.ZipCodes.All()
                    .FirstOrDefaultAsync(x => x.Code.ToString() == zipCode);

                if (targetZipCode == null)
                {
                    targetZipCode = new ZipCode
                    {
                        Code = int.Parse(zipCode),
                        CityId = targetCity?.Id,
                    };

                    await this.unitOfWork.ZipCodes.AddAsync(targetZipCode);
                    await this.unitOfWork.CompleteAsync();
                }
                else
                {
                    if (targetCity != null && targetCity.Id != targetZipCode.CityId)
                    {
                        targetZipCode.CityId = targetCity.Id;

                        this.unitOfWork.ZipCodes.Update(targetZipCode);
                        await this.unitOfWork.CompleteAsync();
                    }
                }

                return targetZipCode;
            }

            return null;
        }
    }
}
