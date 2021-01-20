using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tompany.Services.Data.Contracts
{
    public interface IViewService
    {
        Task AddViewAsync(string tripId);
    }
}
