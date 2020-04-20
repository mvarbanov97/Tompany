using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Services.Mapping;

namespace Tompany.Services.Data
{
    public class UsersService : IUsersService
    {
        private readonly IRepository<ApplicationUser> usersRepository;

        public UsersService(IRepository<ApplicationUser> usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public T GetById<T>(string id)
        {
            var trip = this.usersRepository.All().Where(x => x.Id == id)
                .To<T>().FirstOrDefault();

            return trip;
        }
    }
}
