namespace Tompany.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Tompany.Data;
    using Tompany.Data.Common.Repositories;
    using Tompany.Data.Models;
    using Tompany.Data.Models.Enums;
    using Tompany.Services.Data.Contracts;
    using Tompany.Services.Mapping;
    using Tompany.Web.Infrastructure.Contracts;
    using Tompany.Web.Infrastructure.Hubs;
    using Tompany.Web.ViewModels.Users.ViewModels;

    public class UsersService : IUsersService
    {
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly IUnitOfWork unitOfWork;
        private readonly INotificationService notificationService;

        public UsersService(
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            this.hubContext = hubContext;
            this.notificationService = notificationService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApplicationUserViewModel> ExtractUserInfo(string username, ApplicationUser currentUser)
        {
            var user = await this.unitOfWork.Users.All().FirstOrDefaultAsync(u => u.UserName == username);
            var group = new List<string>() { username, currentUser.UserName };

            var model = new ApplicationUserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                RegisteredOn = user.RegisteredOn,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                AboutMe = user.AboutMe,
                ImageUrl = user.ImageUrl,
                CoverImageUrl = user.CoverImageUrl,
                IsBlocked = user.IsBlocked,
                GitHubUrl = user.GitHubUrl,
                FacebookUrl = user.FacebookUrl,
                InstagramUrl = user.InstagramUrl,
                LinkedinUrl = user.LinkedinUrl,
                TwitterUrl = user.TwitterUrl,
                StackoverflowUrl = user.StackoverflowUrl,
                GroupName = string.Join("->", group.OrderBy(x => x)),
            };

            return model;
        }

        public ApplicationUser GetUserById(string id)
        {
            var user = this.unitOfWork.Users.All().FirstOrDefault(x => x.Id == id);

            return user;
        }

        public T GetUserByUsername<T>(string username)
        {
            var user = this.unitOfWork.Users.All().Where(x => x.UserName == username).To<ApplicationUserViewModel>().FirstOrDefault();

            var userT = AutoMapperConfig.MapperInstance.Map<T>(user);

            return userT;
        }

        public async Task GetUserCars(string userId)
        {
            var cars = this.unitOfWork.Users.All().Where(x => x.Cars.Any(x => x.UserId == userId));
        }

        public async Task AcceptTripRequest(string senderId, ApplicationUser currentUser, string tripId)
        {
            if (currentUser == null)
            {
                throw new NullReferenceException($"There is no user with Id {currentUser.Id}");
            }

            ApplicationUser passenger = this.GetUserById(senderId);

            if (passenger == null)
            {
                throw new NullReferenceException($"There is no passenger with Id {passenger.Id}");
            }

            var trip = await this.unitOfWork.Trips
                .All()
                .Where(x => x.Id == tripId)
                .Include(x => x.TripRequest)
                .Include(x => x.Car)
                .Include(x => x.Passengers)
                .FirstOrDefaultAsync();

            if (trip.Passengers.Count() >= trip.Car.Seats)
            {
                throw new Exception("There is no more free seats avaliable.");
            }

            var tripRequest = trip.TripRequest.Where(x => x.TripId == trip.Id && x.SenderId == passenger.Id).FirstOrDefault();
            tripRequest.RequestStatus = RequestStatus.Accepted;

            trip.Passengers.Add(passenger);

            var notificationId = await this.notificationService.AddAcceptedTripRequestNotification(currentUser.UserName, passenger.UserName, "You have been acceppted for trip", tripId);
            var notification = await this.notificationService.GetNotificationByIdAsync(notificationId);
            await this.hubContext.Clients.User(senderId).SendAsync("VisualizeNotification", notification);

            this.unitOfWork.Trips.Update(trip);
            this.unitOfWork.TripRequests.Update(tripRequest);
            await this.unitOfWork.CompleteAsync();
        }

        public async Task DeclineTripRequest(string senderId, ApplicationUser currentUser, string tripId)
        {
            var trip = await this.unitOfWork.Trips
                .All()
                .Where(x => x.Id == tripId)
                .Include(x => x.TripRequest)
                .Include(x => x.Car)
                .Include(x => x.Passengers)
                .FirstOrDefaultAsync();

            var passenger = this.GetUserById(senderId);
            var tripRequest = trip.TripRequest.Where(x => x.TripId == tripId && x.SenderId == senderId).FirstOrDefault();
            tripRequest.RequestStatus = RequestStatus.Declined;

            var notificationId = await this.notificationService.AddDeclinedTripRequestNotification(currentUser.UserName, passenger.UserName, "You trip request has been declined for trip", tripId);
            var notification = await this.notificationService.GetNotificationByIdAsync(notificationId);
            await this.hubContext.Clients.User(senderId).SendAsync("VisualizeNotification", notification);

            this.unitOfWork.TripRequests.Update(tripRequest);
            await this.unitOfWork.CompleteAsync();
        }

        public bool IsRequestAlreadySent(string senderId, string tripId)
        {
            return this.unitOfWork.TripRequests.All().Any(x => x.SenderId == senderId && x.TripId == tripId);
        }

        public ApplicationUserViewModel GetUserByUsername(string username)
        {
            var user = this.unitOfWork.Users.All().Where(x => x.UserName == username).FirstOrDefaultAsync();

            var userT = AutoMapperConfig.MapperInstance.Map<ApplicationUserViewModel>(user);

            return userT;
        }

        public async Task<bool> IsUserExists(string username)
        {
            return await this.unitOfWork.Users.All().AnyAsync(x => x.UserName == username);
        }

        public async Task<int> TakeCreatedTripPostsCountByUsername(string username)
        {
            return await this.unitOfWork.UserTrips.All().Where(x => x.User.UserName == username).CountAsync();
        }

        public async Task<double> RateUser(ApplicationUser currentUser, string username, int rate)
        {
            var user = await this.unitOfWork.Users.All().FirstOrDefaultAsync(x => x.UserName == username);

            var targetRating = await this.unitOfWork.UserRatings
                .All()
                .FirstOrDefaultAsync(x => x.Username == username && x.RaterUsername == currentUser.UserName);

            if (targetRating != null)
            {
                targetRating.Stars = rate;
                this.unitOfWork.UserRatings.Update(targetRating);
            }
            else
            {
                targetRating = new UserRating
                {
                    RaterUsername = currentUser.UserName,
                    Username = username,
                    Stars = rate,
                };
                await this.unitOfWork.UserRatings.AddAsync(targetRating);
            }

            await this.unitOfWork.CompleteAsync();

            if (currentUser.UserName != username)
            {
                string notificationId =
                       await this.notificationService
                       .AddProfileRatingNotification(user, currentUser, rate);

                var count = await this.notificationService.GetUserNotificationsCount(user.UserName);
                await this.hubContext
                    .Clients
                    .User(user.Id)
                    .SendAsync("ReceiveNotification", count, true);

                var notificationForApproving = await this.notificationService.GetNotificationByIdAsync(notificationId);
                await this.hubContext.Clients.User(user.Id)
                    .SendAsync("VisualizeNotification", notificationForApproving);
            }

            return this.CalculateRatingScore(username);
        }

        public double ExtractUserRatingScore(string username)
        {
            return this.CalculateRatingScore(username);
        }

        public async Task<int> GetLatestScore(ApplicationUser currentUser, string username)
        {
            var target = await this.unitOfWork.UserRatings
                .All()
                .FirstOrDefaultAsync(x => x.Username == username && x.RaterUsername == currentUser.UserName);

            return target == null ? 0 : target.Stars;
        }

        private double CalculateRatingScore(string username)
        {
            double score;
            var count = this.unitOfWork.UserRatings.All().Where(x => x.Username == username).Count();
            if (count != 0)
            {
                var totalScore = this.unitOfWork.UserRatings.All().Where(x => x.Username == username).Sum(x => x.Stars);
                score = Math.Round((double)totalScore / count, 2);

                return score;
            }

            return 0;
        }
    }
}
