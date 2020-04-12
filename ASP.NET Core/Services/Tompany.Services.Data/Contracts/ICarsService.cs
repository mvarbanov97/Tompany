using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Web.ViewModels.Cars;

namespace Tompany.Services.Data.Contracts
{
    public interface ICarsService
    {
        Task CreateAsync(CarCreateInputModel carInputModel);
        IEnumerable<T> GetAll<T>(int? count = null);
    }
}
