using System;
using System.Collections.Generic;
using System.Text;

namespace Tompany.Services.Data.Contracts
{
    public interface IUsersService
    {
        T GetById<T>(string id);
    }
}
