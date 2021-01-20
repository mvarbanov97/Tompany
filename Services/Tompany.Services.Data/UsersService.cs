using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;
using Tompany.Web.Infrastructure.Contracts;
using Tompany.Web.Infrastructure.Hubs;
using Tompany.Web.ViewModels.Users.ViewModels;

namespace Tompany.Services.Data
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Trip> tripsRepository;
        private readonly IRepository<TripRequest> tripRequestRepository;
        private readonly IRepository<UserRating> userRatingRepository;
        private readonly IRepository<UserTrip> userTripRepository;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly INotificationService notificationService;

        public UsersService(
            IRepository<ApplicationUser> usersRepository,
            IRepository<Trip> tripsRepository,
            IRepository<TripRequest> tripRequestRepository,
            IRepository<UserRating> userRatingRepository,
            IRepository<UserTrip> userTripRepository,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService)
        {
            this.usersRepository = usersRepository;
            this.tripsRepository = tripsRepository;
            this.tripRequestRepository = tripRequestRepository;
            this.userRatingRepository = userRatingRepository;
            this.userTripRepository = userTripRepository;
            this.hubContext = hubContext;
            this.notificationService = notificationService;
        }

        public async Task<ApplicationUserViewModel> ExtractUserInfo(string username, ApplicationUser currentUser)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
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
            var user = this.usersRepository.All().FirstOrDefault(x => x.Id == id);

            return user;
        }

        public T GetUserByUsername<T>(string username)
        {
            var user = this.usersRepository.All().Where(x => x.UserName == username).To<ApplicationUserViewModel>().FirstOrDefault();

            var userT = AutoMapperConfig.MapperInstance.Map<T>(user);

            return userT;
        }

        public async Task GetUserCars(string userId)
        {
            var cars = this.usersRepository.All().Where(x => x.Cars.Any(x => x.UserId == userId));
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

            var trip = await this.tripsRepository
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

            this.tripsRepository.Update(trip);
            this.tripRequestRepository.Update(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();
        }

        public async Task DeclineTripRequest(string senderId, ApplicationUser currentUser, string tripId)
        {
            var trip = await this.tripsRepository
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

            this.tripRequestRepository.Update(tripRequest);
            await this.tripRequestRepository.SaveChangesAsync();
        }

        public bool IsRequestAlreadySent(string senderId, string tripId)
        {
            return this.tripRequestRepository.All().Any(x => x.SenderId == senderId && x.TripId == tripId);
        }

        public ApplicationUserViewModel GetUserByUsername(string username)
        {
            var user = this.usersRepository.All().Where(x => x.UserName == username).FirstOrDefaultAsync();

            var userT = AutoMapperConfig.MapperInstance.Map<ApplicationUserViewModel>(user);

            return userT;
        }

        public async Task<bool> IsUserExists(string username)
        {
            return await this.usersRepository.All().AnyAsync(x => x.UserName == username);
        }

        public async Task<int> TakeCreatedTripPostsCountByUsername(string username)
        {
            return await this.userTripRepository.All().Where(x => x.User.UserName == username).CountAsync();
        }

        public async Task<double> RateUser(ApplicationUser currentUser, string username, int rate)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(x => x.UserName == username);

            var targetRating = await this.userRatingRepository
                .All()
                .FirstOrDefaultAsync(x => x.Username == username && x.RaterUsername == currentUser.UserName);

            if (targetRating != null)
            {
                targetRating.Stars = rate;
                this.userRatingRepository.Update(targetRating);
            }
            else
            {
                targetRating = new UserRating
                {
                    RaterUsername = currentUser.UserName,
                    Username = username,
                    Stars = rate,
                };
                await this.userRatingRepository.AddAsync(targetRating);
            }

            await this.userRatingRepository.SaveChangesAsync();

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
            var target = await this.userRatingRepository
                .All()
                .FirstOrDefaultAsync(x => x.Username == username && x.RaterUsername == currentUser.UserName);
            return target == null ? 0 : target.Stars;
        }

        private double CalculateRatingScore(string username)
        {
            double score;
            var count = this.userRatingRepository.All().Where(x => x.Username == username).Count();
            if (count != 0)
            {
                var totalScore = this.userRatingRepository.All().Where(x => x.Username == username).Sum(x => x.Stars);
                score = Math.Round((double)totalScore / count, 2);

                return score;
            }

            return 0;
        }
    }
}
