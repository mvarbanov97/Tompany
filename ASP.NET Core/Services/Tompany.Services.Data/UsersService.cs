using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;

namespace Tompany.Services.Data
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Trip> tripsRepository;

        public UsersService(IRepository<ApplicationUser> usersRepository,
            IRepository<Trip> tripsRepository)
        {
            this.usersRepository = usersRepository;
            this.tripsRepository = tripsRepository;
        }

        public T GetById<T>(string id)
        {
            var user = this.usersRepository.All().Where(x => x.Id == id)
                .To<T>().FirstOrDefault();

            return user;
        }

        public Task<ApplicationUser> GetUserByTripId(string ownerId)
        {
            var user = this.usersRepository.All().FirstOrDefaultAsync(x => x.Id == ownerId);

            return user;
        }
    }
}
