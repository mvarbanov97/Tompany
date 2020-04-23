using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;

namespace Tompany.Services.Data.Contracts
{
    public interface IUsersService
    {
        ApplicationUser GetUserById(string id);
    }
}
