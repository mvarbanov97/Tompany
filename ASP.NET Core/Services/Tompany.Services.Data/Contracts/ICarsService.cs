using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;
using Tompany.Web.ViewModels.Cars.InputModels;

namespace Tompany.Services.Data.Contracts
{
    public interface ICarsService
    {
        Task CreateAsync(string userId, CarCreateInputModel carInputModel);

        Task EditAsync(CarEditIputModel model, string userId);

        Task DeleteAsync(int id, string userId);

        IEnumerable<T> GetAllUserCarsByUserId<T>(string userId);

        IEnumerable<T> GetAllUserCarsByUserUsername<T>(string username);

        T GetCarById<T>(int carId);

        Task<CarEditIputModel> ExtractCar(int id, string userId);

        bool IsCarExists(int id, string userId);
    }
}
